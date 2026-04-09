using System;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Windows.Forms;

namespace ZplPrinter
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;
        private const string SettingsFileName = "user_settings.json";

        // ══════════════════════════════════════════════════════════════════
        // USB RAW 인쇄용 Win32 P/Invoke
        // winspool.drv 를 직접 호출하여 ZPL 바이트를 프린터 스풀러로 전달한다.
        // ══════════════════════════════════════════════════════════════════
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DOCINFO
        {
            [MarshalAs(UnmanagedType.LPTStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPTStr)] public string? pOutputFile;
            [MarshalAs(UnmanagedType.LPTStr)] public string pDataType;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr hPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO di);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        // ── 초기화 ────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
            LoadInstalledPrinters();
            LoadUserSettings();
            // 기본 포커스: 바코드 입력란
            this.Shown += Form1_Shown;
            // 스캐너에서 Enter 키(개행)가 들어올 경우 처리
            txtBarcode.KeyDown += txtBarcode_KeyDown;

            // 변경 시 설정 저장
            cmbPort.SelectedIndexChanged += (s, e) => SaveUserSettings();
            cmbPrinter.SelectedIndexChanged += (s, e) => SaveUserSettings();
            txtIpAddress.TextChanged += (s, e) => SaveUserSettings();
            txtEthPort.TextChanged += (s, e) => SaveUserSettings();
            chkShowBarcode.CheckedChanged += (s, e) => SaveUserSettings();
            chkAutoPrint.CheckedChanged += (s, e) => SaveUserSettings();
            rdoCom.CheckedChanged += (s, e) => SaveUserSettings();
            rdoUsb.CheckedChanged += (s, e) => SaveUserSettings();
            rdoEthernet.CheckedChanged += (s, e) => SaveUserSettings();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txtBarcode.Focus();
        }

        // ── 연결 방식 전환 ─────────────────────────────────────────────────
        private void rdoConnect_CheckedChanged(object sender, EventArgs e)
        {
            pnlCom.Visible = rdoCom.Checked;
            pnlUsb.Visible = rdoUsb.Checked;
            pnlEthernet.Visible = rdoEthernet.Checked;

            // 상태 표시 갱신
            string mode = rdoCom.Checked ? "COM 시리얼"
                        : rdoUsb.Checked ? "USB (RAW)"
                                              : "Ethernet (TCP/IP)";
            SetStatus($"연결 방식: {mode}", success: true);
            // 저장는 개별 이벤트 핸들러에서도 처리하지만 안전하게 한 번 더 저장
            SaveUserSettings();
        }

        private sealed class UserSettings
        {
            public string? ConnectMode { get; set; }
            public string? PortName { get; set; }
            public string? PrinterName { get; set; }
            public string? IpAddress { get; set; }
            public string? EthPort { get; set; }
            public bool ShowBarcode { get; set; }
            public bool AutoPrint { get; set; }
            public bool DoublePrint { get; set; }
            public string? Width { get; set; }
            public string? Height { get; set; }
        }

        private string GetSettingsPath()
            => Path.Combine(Application.UserAppDataPath, SettingsFileName);

        private void SaveUserSettings()
        {
            try
            {
                var s = new UserSettings
                {
                    ConnectMode = rdoCom.Checked ? "COM" : rdoUsb.Checked ? "USB" : "ETH",
                    PortName = cmbPort.SelectedItem?.ToString(),
                    PrinterName = cmbPrinter.SelectedItem?.ToString(),
                    IpAddress = txtIpAddress.Text,
                    EthPort = txtEthPort.Text,
                    ShowBarcode = chkShowBarcode.Checked,
                    AutoPrint = chkAutoPrint.Checked,
                    DoublePrint = chkDoublePrint.Checked,
                    Width = txtWidth.Text,
                    Height = txtHeight.Text
                };

                string dir = Path.GetDirectoryName(GetSettingsPath())!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetSettingsPath(), json, Encoding.UTF8);
            }
            catch
            {
                // 실패해도 앱 동작에는 영향 없도록 숨김
            }
        }

        private void LoadUserSettings()
        {
            try
            {
                string path = GetSettingsPath();
                if (!File.Exists(path))
                    return;

                string json = File.ReadAllText(path, Encoding.UTF8);
                var s = JsonSerializer.Deserialize<UserSettings>(json);
                if (s == null) return;

                // 연결 모드
                if (s.ConnectMode == "COM") rdoCom.Checked = true;
                else if (s.ConnectMode == "USB") rdoUsb.Checked = true;
                else if (s.ConnectMode == "ETH") rdoEthernet.Checked = true;

                // COM 포트가 목록에 있으면 선택
                if (!string.IsNullOrWhiteSpace(s.PortName))
                {
                    for (int i = 0; i < cmbPort.Items.Count; i++)
                    {
                        if (cmbPort.Items[i]?.ToString() == s.PortName)
                        {
                            cmbPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                // 프린터
                if (!string.IsNullOrWhiteSpace(s.PrinterName))
                {
                    for (int i = 0; i < cmbPrinter.Items.Count; i++)
                    {
                        if (cmbPrinter.Items[i]?.ToString() == s.PrinterName)
                        {
                            cmbPrinter.SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(s.IpAddress)) txtIpAddress.Text = s.IpAddress;
                if (!string.IsNullOrWhiteSpace(s.EthPort)) txtEthPort.Text = s.EthPort;
                chkShowBarcode.Checked = s.ShowBarcode;
                chkAutoPrint.Checked = s.AutoPrint;
                chkDoublePrint.Checked = s.DoublePrint;
                if (!string.IsNullOrWhiteSpace(s.Width)) txtWidth.Text = s.Width;
                if (!string.IsNullOrWhiteSpace(s.Height)) txtHeight.Text = s.Height;
            }
            catch
            {
                // ignore
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // COM 시리얼
        // ══════════════════════════════════════════════════════════════════
        private void LoadAvailablePorts()
        {
            cmbPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                cmbPort.Items.AddRange(ports);
                cmbPort.SelectedIndex = 0;
            }
            else
            {
                cmbPort.Items.Add("사용 가능한 포트 없음");
                cmbPort.SelectedIndex = 0;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAvailablePorts();
            SetStatus("COM 포트 목록을 새로고침했습니다.", success: true);
        }

        private bool PrintViaCom(string zpl, string barcodeDisplay, bool showMessageBox = true, int copies = 1)
        {
            if (cmbPort.SelectedItem == null ||
                cmbPort.SelectedItem.ToString() == "사용 가능한 포트 없음")
            {
                ShowError("유효한 COM 포트를 선택해주세요.");
                return false;
            }

            string portName = cmbPort.SelectedItem.ToString()!;
            try
            {
                _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    Encoding = Encoding.ASCII,
                    ReadTimeout = 3000,
                    WriteTimeout = 3000,
                    DtrEnable = true,
                    RtsEnable = true
                };

                _serialPort.Open();

                byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                for (int i = 0; i < copies; i++)
                {
                    _serialPort.BaseStream.Write(bytes, 0, bytes.Length);
                    _serialPort.BaseStream.Flush();
                    // 프린터가 수신하고 처리할 시간
                    System.Threading.Thread.Sleep(120);
                }

                _serialPort.Close();

                SetStatus($"✓ [COM] 인쇄 완료: {portName} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                {
                    MessageBox.Show(this,
                        $"[COM] 포트 {portName} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"COM 인쇄 오류: {ex.Message}");
                _serialPort?.Close();
                return false;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // USB (Windows 스풀러 → RAW 데이터)
        // ══════════════════════════════════════════════════════════════════
        private void LoadInstalledPrinters()
        {
            cmbPrinter.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
                cmbPrinter.Items.Add(printer);

            if (cmbPrinter.Items.Count > 0)
                cmbPrinter.SelectedIndex = 0;
            else
                cmbPrinter.Items.Add("설치된 프린터 없음");
        }

        private void btnRefreshPrinter_Click(object sender, EventArgs e)
        {
            LoadInstalledPrinters();
            SetStatus("프린터 목록을 새로고침했습니다.", success: true);
        }

        private bool PrintViaUsb(string zpl, string barcodeDisplay, bool showMessageBox = true, int copies = 1)
        {
            if (cmbPrinter.SelectedItem == null ||
                cmbPrinter.SelectedItem.ToString() == "설치된 프린터 없음")
            {
                ShowError("유효한 프린터를 선택해주세요.");
                return false;
            }

            string printerName = cmbPrinter.SelectedItem.ToString()!;
            try
            {
                for (int i = 0; i < copies; i++)
                {
                    SendRawToPrinter(printerName, zpl);
                    System.Threading.Thread.Sleep(120);
                }
                SetStatus($"✓ [USB] 인쇄 완료: {printerName} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                {
                    MessageBox.Show(this,
                        $"[USB] 프린터 '{printerName}' 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"USB 인쇄 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// winspool.drv를 통해 ZPL 문자열을 RAW 형식으로 프린터에 직접 전송한다.
        /// ZPL 프린터는 Windows GDI 렌더링을 우회해야 하므로 pDataType = "RAW" 필수.
        /// </summary>
        private static void SendRawToPrinter(string printerName, string zpl)
        {
            if (!OpenPrinter(printerName, out IntPtr hPrinter, IntPtr.Zero))
                throw new InvalidOperationException(
                    $"프린터를 열 수 없습니다: '{printerName}'\n" +
                    $"(Win32 오류코드: {Marshal.GetLastWin32Error()})");
            try
            {
                var di = new DOCINFO
                {
                    pDocName = "ZPL Print Job",
                    pOutputFile = null,
                    pDataType = "RAW"          // ZPL을 GDI 변환 없이 그대로 전달
                };

                if (!StartDocPrinter(hPrinter, 1, ref di))
                    throw new InvalidOperationException("인쇄 작업(DocPrinter)을 시작할 수 없습니다.");
                try
                {
                    if (!StartPagePrinter(hPrinter))
                        throw new InvalidOperationException("페이지 인쇄(PagePrinter)를 시작할 수 없습니다.");

                    byte[] bytes = Encoding.UTF8.GetBytes(zpl);
                    IntPtr pBytes = Marshal.AllocHGlobal(bytes.Length);
                    try
                    {
                        Marshal.Copy(bytes, 0, pBytes, bytes.Length);
                        if (!WritePrinter(hPrinter, pBytes, bytes.Length, out int written) || written == 0)
                            throw new InvalidOperationException("프린터에 데이터를 쓸 수 없습니다.");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pBytes);
                    }
                    EndPagePrinter(hPrinter);
                }
                finally
                {
                    EndDocPrinter(hPrinter);
                }
            }
            finally
            {
                ClosePrinter(hPrinter);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // Ethernet (TCP/IP 소켓)
        // ══════════════════════════════════════════════════════════════════
        private bool PrintViaEthernet(string zpl, string barcodeDisplay, bool showMessageBox = true, int copies = 1)
        {
            string ip = txtIpAddress.Text.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                ShowError("IP 주소를 입력해주세요.");
                return false;
            }

            if (!int.TryParse(txtEthPort.Text.Trim(), out int port) || port < 1 || port > 65535)
            {
                ShowError("올바른 포트 번호를 입력해주세요. (1 ~ 65535)");
                return false;
            }

            try
            {
                for (int i = 0; i < copies; i++)
                {
                    using var client = new TcpClient();

                    // 5초 타임아웃으로 연결 시도
                    bool connected = client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(5));
                    if (!connected)
                        throw new TimeoutException($"연결 시간 초과 ({ip}:{port})\nIP 주소와 포트를 확인해주세요.");

                    using var stream = client.GetStream();
                    byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();

                    // 프린터가 처리할 시간
                    System.Threading.Thread.Sleep(120);
                }

                SetStatus($"✓ [Ethernet] 인쇄 완료: {ip}:{port} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                {
                    MessageBox.Show(this,
                        $"[Ethernet] {ip}:{port} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Ethernet 인쇄 오류: {ex.Message}");
                return false;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // 인쇄 버튼 (공통 진입점)
        // ══════════════════════════════════════════════════════════════════
        private void btnPrint_Click(object sender, EventArgs e)
        {
            // ── 입력값 검증 ───────────────────────────────────────────────
            string barcode = txtBarcode.Text.Trim();
            if (barcode.Length != 12 || !long.TryParse(barcode, out _))
            {
                ShowError("바코드는 12자리 숫자여야 합니다.");
                return;
            }

            if (!double.TryParse(txtWidth.Text, out double labelWidth) || labelWidth <= 0)
            {
                ShowError("올바른 라벨 가로 크기를 입력해주세요.");
                return;
            }

            if (!double.TryParse(txtHeight.Text, out double labelHeight) || labelHeight <= 0)
            {
                ShowError("올바른 라벨 세로 크기를 입력해주세요.");
                return;
            }

            // ── ZPL 생성 ──────────────────────────────────────────────────
            string barcodeDisplay = FormatBarcodeDisplay(barcode);
            string zpl = BuildZpl(barcode, barcodeDisplay, chkShowBarcode.Checked);

            // ── 미리보기 ──────────────────────────────────────────────────
            using var preview = new PrintPreviewForm(
                zpl, barcodeDisplay, barcode, labelWidth, labelHeight, chkShowBarcode.Checked);
            if (preview.ShowDialog() != DialogResult.OK)
                return;

            // ── 연결 방식별 전송 ──────────────────────────────────────────
            int copies = chkDoublePrint.Checked ? 2 : 1;
            if (rdoCom.Checked) _ = PrintViaCom(zpl, barcodeDisplay, showMessageBox: true, copies: copies);
            else if (rdoUsb.Checked) _ = PrintViaUsb(zpl, barcodeDisplay, showMessageBox: true, copies: copies);
            else _ = PrintViaEthernet(zpl, barcodeDisplay, showMessageBox: true, copies: copies);
        }

        // Enter 키(스캐너 개행) 이벤트 처리
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.Handled = true;
            e.SuppressKeyPress = true; // 텍스트박스에 Enter가 삽입되는 것을 방지

            // 공백 및 제어문자 제거
            txtBarcode.Text = txtBarcode.Text.Trim();

            if (chkAutoPrint.Checked)
            {
                PerformDirectPrint();
            }
            else
            {
                // 일반 모드: 기존 버튼 클릭 로직을 호출하여 미리보기 표시
                btnPrint_Click(this, EventArgs.Empty);
            }
        }

        // 자동 발행 모드: 미리보기 없이 바로 프린트 전송
        private void PerformDirectPrint()
        {
            // 입력값 검증 (버튼에서 사용된 로직 복제)
            string barcode = txtBarcode.Text.Trim();
            if (barcode.Length != 12 || !long.TryParse(barcode, out _))
            {
                ShowError("바코드는 12자리 숫자여야 합니다.");
                return;
            }

            if (!double.TryParse(txtWidth.Text, out double labelWidth) || labelWidth <= 0)
            {
                ShowError("올바른 라벨 가로 크기를 입력해주세요.");
                return;
            }

            if (!double.TryParse(txtHeight.Text, out double labelHeight) || labelHeight <= 0)
            {
                ShowError("올바른 라벨 세로 크기를 입력해주세요.");
                return;
            }

            string barcodeDisplay = FormatBarcodeDisplay(barcode);
            string zpl = BuildZpl(barcode, barcodeDisplay, chkShowBarcode.Checked);

            // 바로 전송 (자동 발행: MessageBox 숨김)
            bool success;
            int copies = chkDoublePrint.Checked ? 2 : 1;
            if (rdoCom.Checked) success = PrintViaCom(zpl, barcodeDisplay, showMessageBox: false, copies: copies);
            else if (rdoUsb.Checked) success = PrintViaUsb(zpl, barcodeDisplay, showMessageBox: false, copies: copies);
            else success = PrintViaEthernet(zpl, barcodeDisplay, showMessageBox: false, copies: copies);

            if (success)
            {
                // 발행 성공 시 다음 바코드 입력을 위해 텍스트박스 비우고 포커스 유지
                txtBarcode.Clear();
                txtBarcode.Focus();
            }
        }

        // ── ZPL 빌더 ─────────────────────────────────────────────────────
        private static string FormatBarcodeDisplay(string raw)
            => $"{raw[..3]}-{raw.Substring(3, 2)}-{raw[5..]}";

        private static string BuildZpl(string barcode, string barcodeDisplay, bool showBarcode)
        {
            if (showBarcode)
            {
                return
$@"^XA
^FO360,180
^A0,50,50^FD{barcodeDisplay}^FS
^FO200,60
^BY4
^BCN,100,N,N,N^FD{barcode}^FS
^RFW,H,1,2,1^FD3000^FS
^RFW,A,2,12,1^FD{barcode}^FS
^RFR,A,^FN1^FS
^FH_^HV1,,EPC-Ascii  DATA:[,]_0D_0A^FS
^PQ1
^XZ";
            }
            else
            {
                return
$@"^XA
^RFW,H,1,2,1^FD3000^FS
^RFW,A,2,12,1^FD{barcode}^FS
^RFR,A,^FN1^FS
^FH_^HV1,,EPC-Ascii  DATA:[,]_0D_0A^FS
^PQ1
^XZ";
            }
        }

        // ── 상태 표시 / 오류 ─────────────────────────────────────────────
        private void SetStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = success
                ? System.Drawing.Color.FromArgb(0, 180, 120)
                : System.Drawing.Color.FromArgb(220, 60, 60);
        }

        private void ShowError(string message)
        {
            SetStatus($"✗ {message}", success: false);
            MessageBox.Show(message, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
            if (char.IsDigit(e.KeyChar) && txtBarcode.Text.Length >= 12)
                e.Handled = true;
        }
    }
}