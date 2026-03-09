using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ZplPrinter
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;

        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
        }

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
            lblStatus.Text = "포트 목록을 새로고침했습니다.";
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 180, 120);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (cmbPort.SelectedItem == null || cmbPort.SelectedItem.ToString() == "사용 가능한 포트 없음")
            {
                ShowError("유효한 COM 포트를 선택해주세요.");
                return;
            }

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

            using var preview = new PrintPreviewForm(zpl, barcodeDisplay, barcode, labelWidth, labelHeight, chkShowBarcode.Checked);
            if (preview.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string portName = cmbPort.SelectedItem.ToString()!;
                _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Open();
                _serialPort.Write(zpl);
                _serialPort.Close();

                lblStatus.Text = $"✓ 인쇄 완료: {portName} → {barcodeDisplay}";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 180, 120);

                using var result = new PrintResultForm(barcodeDisplay, barcode, portName);
                result.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowError($"인쇄 오류: {ex.Message}");
                _serialPort?.Close();
            }
        }

        private static string FormatBarcodeDisplay(string raw)
        {
            // 252190021426 → 252-19-0021426
            return $"{raw[..3]}-{raw.Substring(3, 2)}-{raw[5..]}";
        }

        private static string BuildZpl(string barcode, string barcodeDisplay, bool showBarcode)
        {
            if (showBarcode)
            {
                // 체크 ON: 바코드 + 포맷 텍스트 출력, BC Human Readable은 항상 N
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
^XZ
";
            }
            else
            {
                // 체크 OFF: 인코딩만, 바코드/텍스트 출력 없음
                return
$@"^XA
^RFW,H,1,2,1^FD3000^FS
^RFW,A,2,12,1^FD{barcode}^FS
^RFR,A,^FN1^FS
^FH_^HV1,,EPC-Ascii  DATA:[,]_0D_0A^FS
^PQ1
^XZ
";
            }
        }

        private void ShowError(string message)
        {
            lblStatus.Text = $"✗ {message}";
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(220, 60, 60);
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