using System;
using System.Collections.Generic;
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

        // 스캔된 바코드 목록 (중복 없음, 삽입 순서 유지)
        private readonly List<string> _scannedBarcodes = new();

        // ══════════════════════════════════════════════════════════════════
        // USB RAW 인쇄용 Win32 P/Invoke
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

            this.Shown += Form1_Shown;
            txtBarcode.KeyDown += txtBarcode_KeyDown;

            // 설정 변경 시 자동 저장
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

        private void Form1_Shown(object sender, EventArgs e) => txtBarcode.Focus();

        // ── 연결 방식 전환 ─────────────────────────────────────────────────
        private void rdoConnect_CheckedChanged(object sender, EventArgs e)
        {
            pnlCom.Visible = rdoCom.Checked;
            pnlUsb.Visible = rdoUsb.Checked;
            pnlEthernet.Visible = rdoEthernet.Checked;

            string mode = rdoCom.Checked ? "COM 시리얼"
                        : rdoUsb.Checked ? "USB (RAW)"
                                               : "Ethernet (TCP/IP)";
            SetStatus($"연결 방식: {mode}", success: true);
            SaveUserSettings();
        }

        // ══════════════════════════════════════════════════════════════════
        // 설정 저장 / 불러오기
        // ══════════════════════════════════════════════════════════════════
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
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetSettingsPath(), json, Encoding.UTF8);
            }
            catch { /* 실패해도 앱 동작에 영향 없음 */ }
        }

        private void LoadUserSettings()
        {
            try
            {
                string path = GetSettingsPath();
                if (!File.Exists(path)) return;

                var s = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(path, Encoding.UTF8));
                if (s == null) return;

                if (s.ConnectMode == "COM") rdoCom.Checked = true;
                else if (s.ConnectMode == "USB") rdoUsb.Checked = true;
                else if (s.ConnectMode == "ETH") rdoEthernet.Checked = true;

                SelectComboItem(cmbPort, s.PortName);
                SelectComboItem(cmbPrinter, s.PrinterName);

                if (!string.IsNullOrWhiteSpace(s.IpAddress)) txtIpAddress.Text = s.IpAddress;
                if (!string.IsNullOrWhiteSpace(s.EthPort)) txtEthPort.Text = s.EthPort;

                chkShowBarcode.Checked = s.ShowBarcode;
                chkAutoPrint.Checked = s.AutoPrint;
                chkDoublePrint.Checked = s.DoublePrint;

                if (!string.IsNullOrWhiteSpace(s.Width)) txtWidth.Text = s.Width;
                if (!string.IsNullOrWhiteSpace(s.Height)) txtHeight.Text = s.Height;
            }
            catch { /* ignore */ }
        }

        private static void SelectComboItem(ComboBox cmb, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (cmb.Items[i]?.ToString() == value) { cmb.SelectedIndex = i; return; }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // COM 시리얼
        // ══════════════════════════════════════════════════════════════════
        private void LoadAvailablePorts()
        {
            cmbPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0) { cmbPort.Items.AddRange(ports); cmbPort.SelectedIndex = 0; }
            else { cmbPort.Items.Add("사용 가능한 포트 없음"); cmbPort.SelectedIndex = 0; }
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
                    System.Threading.Thread.Sleep(120);
                }
                _serialPort.Close();

                SetStatus($"✓ [COM] 인쇄 완료: {portName} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                    MessageBox.Show(this,
                        $"[COM] 포트 {portName} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            if (cmbPrinter.Items.Count > 0) cmbPrinter.SelectedIndex = 0;
            else cmbPrinter.Items.Add("설치된 프린터 없음");
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
                    MessageBox.Show(this,
                        $"[USB] 프린터 '{printerName}' 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var di = new DOCINFO { pDocName = "ZPL Print Job", pOutputFile = null, pDataType = "RAW" };

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
                    finally { Marshal.FreeHGlobal(pBytes); }
                    EndPagePrinter(hPrinter);
                }
                finally { EndDocPrinter(hPrinter); }
            }
            finally { ClosePrinter(hPrinter); }
        }

        // ══════════════════════════════════════════════════════════════════
        // Ethernet (TCP/IP 소켓)
        // ══════════════════════════════════════════════════════════════════
        private bool PrintViaEthernet(string zpl, string barcodeDisplay, bool showMessageBox = true, int copies = 1)
        {
            string ip = txtIpAddress.Text.Trim();
            if (string.IsNullOrEmpty(ip)) { ShowError("IP 주소를 입력해주세요."); return false; }

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
                    bool connected = client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(5));
                    if (!connected)
                        throw new TimeoutException($"연결 시간 초과 ({ip}:{port})\nIP 주소와 포트를 확인해주세요.");

                    using var stream = client.GetStream();
                    byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                    System.Threading.Thread.Sleep(120);
                }

                SetStatus($"✓ [Ethernet] 인쇄 완료: {ip}:{port} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                    MessageBox.Show(this,
                        $"[Ethernet] {ip}:{port} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        // ──────────────────────────────────────────────────────────────────
        //  · 자동 발행 ON  → 텍스트박스의 바코드 1장 미리보기 후 인쇄
        //  · 자동 발행 OFF → 스캔된 목록 전체를 일괄 인쇄 후 목록 초기화
        // ══════════════════════════════════════════════════════════════════
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (chkAutoPrint.Checked)
                PrintSingleWithPreview();
            else
                PrintScannedList();
        }

        // ── 단일 바코드 미리보기 인쇄 (자동 발행 ON) ──────────────────────
        private void PrintSingleWithPreview()
        {
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

            using var preview = new PrintPreviewForm(
                zpl, barcodeDisplay, barcode, labelWidth, labelHeight, chkShowBarcode.Checked);
            if (preview.ShowDialog() != DialogResult.OK) return;

            int copies = chkDoublePrint.Checked ? 2 : 1;
            if (rdoCom.Checked) PrintViaCom(zpl, barcodeDisplay, showMessageBox: true, copies: copies);
            else if (rdoUsb.Checked) PrintViaUsb(zpl, barcodeDisplay, showMessageBox: true, copies: copies);
            else PrintViaEthernet(zpl, barcodeDisplay, showMessageBox: true, copies: copies);
        }

        // ── 스캔 목록 일괄 인쇄 (자동 발행 OFF) ──────────────────────────
        private void PrintScannedList()
        {
            if (_scannedBarcodes.Count == 0)
            {
                ShowError("인쇄할 바코드가 없습니다.\n바코드를 먼저 스캔해주세요.");
                return;
            }

            if (!double.TryParse(txtWidth.Text, out double labelWidth) || labelWidth <= 0 ||
                !double.TryParse(txtHeight.Text, out double labelHeight) || labelHeight <= 0)
            {
                ShowError("올바른 라벨 크기를 입력해주세요.");
                return;
            }

            int total = _scannedBarcodes.Count;
            int copies = chkDoublePrint.Checked ? 2 : 1;
            string copiesMsg = copies > 1 ? $"  (각 {copies}장)" : "";

            var confirm = MessageBox.Show(
                $"총 {total}개 바코드를 인쇄합니다{copiesMsg}.\n계속하시겠습니까?",
                "인쇄 확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm != DialogResult.OK) return;

            int successCount = 0;
            foreach (string barcode in _scannedBarcodes)
            {
                string display = FormatBarcodeDisplay(barcode);
                string zpl = BuildZpl(barcode, display, chkShowBarcode.Checked);

                bool ok = rdoCom.Checked ? PrintViaCom(zpl, display, showMessageBox: false, copies: copies)
                        : rdoUsb.Checked ? PrintViaUsb(zpl, display, showMessageBox: false, copies: copies)
                                               : PrintViaEthernet(zpl, display, showMessageBox: false, copies: copies);
                if (ok)
                {
                    successCount++;
                }
                else
                {
                    // 오류 발생: 성공한 항목은 목록에서 제거하고 중단
                    MessageBox.Show(
                        $"{successCount}/{total}개 인쇄 후 오류가 발생했습니다.\n" +
                        $"나머지 {total - successCount}개는 목록에 유지됩니다.",
                        "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _scannedBarcodes.RemoveRange(0, successCount);
                    UpdateScanListUI();
                    return;
                }
            }

            // 전체 성공
            SetStatus($"✓ {total}개 바코드 인쇄 완료", success: true);
            MessageBox.Show(
                $"{total}개 바코드 인쇄가 완료되었습니다.",
                "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _scannedBarcodes.Clear();
            UpdateScanListUI();
        }

        // ══════════════════════════════════════════════════════════════════
        // 스캔 목록 관리
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// 검증된 12자리 바코드를 목록에 추가한다. 중복이면 무시하고 상태를 알린다.
        /// </summary>
        private void AddToScanList(string barcode)
        {
            if (_scannedBarcodes.Contains(barcode))
            {
                SetStatus($"⚠ 중복 바코드 무시됨: {FormatBarcodeDisplay(barcode)}", success: false);
                return;
            }
            _scannedBarcodes.Add(barcode);
            UpdateScanListUI();
            SetStatus($"✓ 목록 추가: {FormatBarcodeDisplay(barcode)}  (총 {_scannedBarcodes.Count}개)", success: true);
        }

        /// <summary>
        /// ListBox 와 카운트 라벨을 현재 _scannedBarcodes 상태와 동기화한다.
        /// </summary>
        private void UpdateScanListUI()
        {
            lstBarcodes.BeginUpdate();
            lstBarcodes.Items.Clear();
            foreach (string bc in _scannedBarcodes)
                lstBarcodes.Items.Add(FormatBarcodeDisplay(bc));
            lstBarcodes.EndUpdate();

            lblScanCount.Text = $"{_scannedBarcodes.Count} 개";
        }

        /// <summary>
        /// 목록 초기화 버튼 클릭 — 확인 후 목록을 비운다.
        /// </summary>
        private void btnClearList_Click(object sender, EventArgs e)
        {
            if (_scannedBarcodes.Count == 0) return;

            var result = MessageBox.Show(
                $"스캔된 바코드 {_scannedBarcodes.Count}개를 모두 삭제하시겠습니까?",
                "목록 초기화", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result != DialogResult.OK) return;

            _scannedBarcodes.Clear();
            UpdateScanListUI();
            SetStatus("목록이 초기화되었습니다.", success: true);
        }

        // ══════════════════════════════════════════════════════════════════
        // 바코드 텍스트박스 키 이벤트
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Enter 키(스캐너 개행 포함) 처리.
        ///  · 자동 발행 ON  → 즉시 인쇄 (PerformDirectPrint)
        ///  · 자동 발행 OFF → 스캔 목록에 추가
        /// </summary>
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.Handled = true;
            e.SuppressKeyPress = true; // 텍스트박스에 개행 삽입 방지

            string barcode = txtBarcode.Text.Trim();

            if (chkAutoPrint.Checked)
            {
                PerformDirectPrint();
                return;
            }

            // 자동 발행 OFF → 목록에 추가
            if (barcode.Length != 12 || !long.TryParse(barcode, out _))
            {
                ShowError("바코드는 12자리 숫자여야 합니다.");
                return;
            }

            AddToScanList(barcode);
            txtBarcode.Clear();
            txtBarcode.Focus();
        }

        /// <summary>
        /// 자동 발행 모드: 미리보기 없이 즉시 프린터로 전송.
        /// </summary>
        private void PerformDirectPrint()
        {
            string barcode = txtBarcode.Text.Trim();
            if (barcode.Length != 12 || !long.TryParse(barcode, out _))
            {
                ShowError("바코드는 12자리 숫자여야 합니다.");
                return;
            }

            if (!double.TryParse(txtWidth.Text, out double labelWidth) || labelWidth <= 0 ||
                !double.TryParse(txtHeight.Text, out double labelHeight) || labelHeight <= 0)
            {
                ShowError("올바른 라벨 크기를 입력해주세요.");
                return;
            }

            string barcodeDisplay = FormatBarcodeDisplay(barcode);
            string zpl = BuildZpl(barcode, barcodeDisplay, chkShowBarcode.Checked);

            int copies = chkDoublePrint.Checked ? 2 : 1;
            bool success = rdoCom.Checked ? PrintViaCom(zpl, barcodeDisplay, showMessageBox: false, copies: copies)
                         : rdoUsb.Checked ? PrintViaUsb(zpl, barcodeDisplay, showMessageBox: false, copies: copies)
                                                : PrintViaEthernet(zpl, barcodeDisplay, showMessageBox: false, copies: copies);

            if (success)
            {
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
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
            if (char.IsDigit(e.KeyChar) && txtBarcode.Text.Length >= 12) e.Handled = true;
        }
    }
}