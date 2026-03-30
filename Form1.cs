using System;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ZplPrinter
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;

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

        private void PrintViaCom(string zpl, string barcodeDisplay)
        {
            if (cmbPort.SelectedItem == null ||
                cmbPort.SelectedItem.ToString() == "사용 가능한 포트 없음")
            {
                ShowError("유효한 COM 포트를 선택해주세요.");
                return;
            }

            string portName = cmbPort.SelectedItem.ToString()!;
            try
            {
                _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Open();
                _serialPort.Write(zpl);
                _serialPort.Close();

                SetStatus($"✓ [COM] 인쇄 완료: {portName} → {barcodeDisplay}", success: true);
                MessageBox.Show(this,
                    $"[COM] 포트 {portName} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                    "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"COM 인쇄 오류: {ex.Message}");
                _serialPort?.Close();
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

        private void PrintViaUsb(string zpl, string barcodeDisplay)
        {
            if (cmbPrinter.SelectedItem == null ||
                cmbPrinter.SelectedItem.ToString() == "설치된 프린터 없음")
            {
                ShowError("유효한 프린터를 선택해주세요.");
                return;
            }

            string printerName = cmbPrinter.SelectedItem.ToString()!;
            try
            {
                SendRawToPrinter(printerName, zpl);
                SetStatus($"✓ [USB] 인쇄 완료: {printerName} → {barcodeDisplay}", success: true);
                MessageBox.Show(this,
                    $"[USB] 프린터 '{printerName}' 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                    "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"USB 인쇄 오류: {ex.Message}");
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
        private void PrintViaEthernet(string zpl, string barcodeDisplay)
        {
            string ip = txtIpAddress.Text.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                ShowError("IP 주소를 입력해주세요.");
                return;
            }

            if (!int.TryParse(txtEthPort.Text.Trim(), out int port) || port < 1 || port > 65535)
            {
                ShowError("올바른 포트 번호를 입력해주세요. (1 ~ 65535)");
                return;
            }

            try
            {
                using var client = new TcpClient();

                // 5초 타임아웃으로 연결 시도
                bool connected = client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(5));
                if (!connected)
                    throw new TimeoutException($"연결 시간 초과 ({ip}:{port})\nIP 주소와 포트를 확인해주세요.");

                using var stream = client.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(zpl);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                SetStatus($"✓ [Ethernet] 인쇄 완료: {ip}:{port} → {barcodeDisplay}", success: true);
                MessageBox.Show(this,
                    $"[Ethernet] {ip}:{port} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                    "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Ethernet 인쇄 오류: {ex.Message}");
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
            if (rdoCom.Checked) PrintViaCom(zpl, barcodeDisplay);
            else if (rdoUsb.Checked) PrintViaUsb(zpl, barcodeDisplay);
            else PrintViaEthernet(zpl, barcodeDisplay);
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