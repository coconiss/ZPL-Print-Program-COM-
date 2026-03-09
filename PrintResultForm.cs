using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZplPrinter
{
    /// <summary>
    /// 인쇄 완료 후 결과 확인 창
    /// </summary>
    public class PrintResultForm : Form
    {
        private readonly string _barcodeDisplay;
        private readonly string _barcodeRaw;
        private readonly string _portName;

        public PrintResultForm(string barcodeDisplay, string barcodeRaw, string portName)
        {
            _barcodeDisplay = barcodeDisplay;
            _barcodeRaw     = barcodeRaw;
            _portName       = portName;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "인쇄 결과 확인";
            this.Size = new Size(480, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(18, 18, 28);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ── Success icon area ─────────────────────────────────────────
            var pnlIcon = new Panel
            {
                BackColor = Color.FromArgb(0, 160, 100),
                Location = new Point(0, 0),
                Size = new Size(480, 100)
            };

            var lblIcon = new Label
            {
                Text = "✓",
                Font = new Font("Segoe UI", 36f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblSuccessTitle = new Label
            {
                Text = "인쇄 전송 완료",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 16),
                AutoSize = true
            };

            var lblSuccessSub = new Label
            {
                Text = $"포트 {_portName} 으로 ZPL 커멘드가 성공적으로 전송되었습니다.",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(200, 240, 220),
                Location = new Point(80, 58),
                AutoSize = true
            };

            pnlIcon.Controls.AddRange(new Control[] { lblIcon, lblSuccessTitle, lblSuccessSub });

            // ── Result details ────────────────────────────────────────────
            var pnlDetails = new Panel
            {
                BackColor = Color.FromArgb(26, 26, 42),
                Location = new Point(20, 116),
                Size = new Size(440, 150)
            };

            AddDetailRow(pnlDetails, "전송 포트",     _portName,        Color.FromArgb(255, 180, 60),  0);
            AddDetailRow(pnlDetails, "바코드 (RAW)",  _barcodeRaw,      Color.FromArgb(100, 210, 255), 40);
            AddDetailRow(pnlDetails, "출력 표기",     _barcodeDisplay,  Color.FromArgb(100, 210, 255), 80);
            AddDetailRow(pnlDetails, "전송 시각",     DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                        Color.FromArgb(160, 170, 200), 120);

            // ── Question ──────────────────────────────────────────────────
            var lblQuestion = new Label
            {
                Text = "라벨이 올바르게 인쇄되었나요?",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 210, 230),
                Location = new Point(20, 278),
                AutoSize = true
            };

            // ── Buttons ───────────────────────────────────────────────────
            var btnOk = new Button
            {
                Text = "✓  정상 인쇄됨",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 140, 90),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 308),
                Size = new Size(140, 40),
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnRetry = new Button
            {
                Text = "↻  다시 인쇄",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 100, 180),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(176, 308),
                Size = new Size(130, 40),
                DialogResult = DialogResult.Retry,
                Cursor = Cursors.Hand
            };
            btnRetry.FlatAppearance.BorderSize = 0;

            var btnFail = new Button
            {
                Text = "✗  인쇄 실패",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(160, 40, 40),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(322, 308),
                Size = new Size(130, 40),
                DialogResult = DialogResult.Abort,
                Cursor = Cursors.Hand
            };
            btnFail.FlatAppearance.BorderSize = 0;

            btnRetry.Click += (s, e) =>
            {
                MessageBox.Show("메인 화면에서 '인쇄 전송' 버튼을 다시 눌러 재시도하세요.",
                    "재인쇄 안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.Retry;
                this.Close();
            };

            btnFail.Click += (s, e) =>
            {
                MessageBox.Show(
                    "인쇄 오류가 발생한 경우 다음을 확인하세요:\n\n" +
                    "1. 프린터 전원 및 케이블 연결 상태\n" +
                    "2. 선택한 COM 포트 번호 일치 여부\n" +
                    "3. Baud Rate / 통신 파라미터 설정\n" +
                    "4. 라벨지 및 리본 잔량",
                    "인쇄 실패 - 점검 안내",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Abort;
                this.Close();
            };

            this.AcceptButton = btnOk;

            this.Controls.AddRange(new Control[] {
                pnlIcon, pnlDetails, lblQuestion, btnOk, btnRetry, btnFail
            });
        }

        private static void AddDetailRow(Panel panel, string label, string value, Color valueColor, int y)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(140, 150, 170),
                Location = new Point(14, y + 12),
                Size = new Size(110, 22)
            };
            var val = new Label
            {
                Text = value,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = valueColor,
                Location = new Point(130, y + 12),
                AutoSize = true
            };
            panel.Controls.AddRange(new Control[] { lbl, val });
        }
    }
}
