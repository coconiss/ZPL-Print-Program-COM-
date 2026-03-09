using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZplPrinter
{
    /// <summary>
    /// ZPL 전송 전 내용을 확인하는 미리보기 창
    /// </summary>
    public class PrintPreviewForm : Form
    {
        private readonly string _zpl;
        private readonly string _barcodeDisplay;
        private readonly string _barcodeRaw;
        private readonly double _labelWidth;
        private readonly double _labelHeight;
        private readonly bool _showText;

        public PrintPreviewForm(string zpl, string barcodeDisplay, string barcodeRaw,
            double labelWidth, double labelHeight, bool showText)
        {
            _zpl = zpl;
            _barcodeDisplay = barcodeDisplay;
            _barcodeRaw = barcodeRaw;
            _labelWidth = labelWidth;
            _labelHeight = labelHeight;
            _showText = showText;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "인쇄 미리보기 확인";
            this.Size = new Size(640, 660);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(18, 18, 28);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ── Header ────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                BackColor = Color.FromArgb(26, 26, 42),
                Location = new Point(0, 0),
                Size = new Size(640, 56)
            };
            var lblTitle = new Label
            {
                Text = "인쇄 전 내용을 확인해주세요",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 210, 255),
                Location = new Point(20, 14),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);

            // ── Info Table ────────────────────────────────────────────────
            var pnlInfo = new Panel
            {
                BackColor = Color.FromArgb(26, 26, 42),
                Location = new Point(16, 68),
                Size = new Size(608, 140),
                Padding = new Padding(12)
            };

            AddInfoRow(pnlInfo, "바코드 (RAW):", _barcodeRaw, Color.FromArgb(100, 210, 255), 0);
            AddInfoRow(pnlInfo, "출력 표기:", _barcodeDisplay, Color.FromArgb(100, 210, 255), 34);
            AddInfoRow(pnlInfo, "라벨 크기:", $"{_labelWidth:F1} × {_labelHeight:F1} mm  (300 DPI)", Color.FromArgb(255, 200, 80), 68);
            AddInfoRow(pnlInfo, "출력 모드:",
                _showText ? "바코드 + 텍스트 출력" : "인코딩 전용 (화면 출력 없음)",
                _showText ? Color.FromArgb(0, 200, 120) : Color.FromArgb(255, 160, 40), 102);

            // ── Label canvas preview ───────────────────────────────────────
            var lblPreviewTitle = new Label
            {
                Text = "라벨 미리보기 (시각적 참고용)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 130, 160),
                Location = new Point(16, 220),
                AutoSize = true
            };

            var canvas = new LabelCanvas(_barcodeDisplay, _barcodeRaw, _labelWidth, _labelHeight, _showText)
            {
                Location = new Point(16, 244),
                Size = new Size(608, 240)
            };

            // ── ZPL raw view (collapsible) ────────────────────────────────
            var btnToggle = new Button
            {
                Text = "▾  ZPL 커멘드 보기",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 44, 60),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(16, 496),
                Size = new Size(160, 26),
                Cursor = Cursors.Hand
            };
            btnToggle.FlatAppearance.BorderSize = 0;

            var txtZpl = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.FromArgb(12, 12, 22),
                ForeColor = Color.FromArgb(120, 220, 100),
                Font = new Font("Consolas", 8.5f),
                Location = new Point(16, 496),
                Size = new Size(608, 0),
                Text = _zpl,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            bool zplVisible = false;
            btnToggle.Click += (s, e) =>
            {
                zplVisible = !zplVisible;
                txtZpl.Visible = zplVisible;
                txtZpl.Size = zplVisible ? new Size(608, 80) : new Size(608, 0);
                btnToggle.Text = zplVisible ? "▴  ZPL 커멘드 숨기기" : "▾  ZPL 커멘드 보기";
            };

            // ── Buttons ───────────────────────────────────────────────────
            var pnlBtns = new Panel
            {
                BackColor = Color.FromArgb(22, 22, 36),
                Location = new Point(0, 548),
                Size = new Size(640, 60)
            };

            var btnOk = new Button
            {
                Text = "✓  인쇄 전송",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 120, 220),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(380, 10),
                Size = new Size(120, 40),
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "✗  취소",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 40, 40),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(516, 10),
                Size = new Size(100, 40),
                DialogResult = DialogResult.Cancel,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            pnlBtns.Controls.AddRange(new Control[] { btnOk, btnCancel });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            this.Controls.AddRange(new Control[] {
                pnlHeader, pnlInfo, lblPreviewTitle, canvas,
                btnToggle, txtZpl, pnlBtns
            });
        }

        private static void AddInfoRow(Panel panel, string label, string value, Color valueColor, int y)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(140, 150, 170),
                Location = new Point(12, y + 10),
                Size = new Size(140, 24)
            };
            var val = new Label
            {
                Text = value,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = valueColor,
                Location = new Point(155, y + 10),
                AutoSize = true
            };
            panel.Controls.AddRange(new Control[] { lbl, val });
        }
    }

    /// <summary>
    /// 라벨을 시각적으로 그려주는 커스텀 컨트롤
    /// ZPL 좌표 기준: 바코드(Y=60 위쪽) → Human Readable → 포맷 텍스트(Y=180 아래쪽)
    /// </summary>
    public class LabelCanvas : Control
    {
        private readonly string _display;   // 252-19-0021426
        private readonly string _raw;       // 252190021426
        private readonly double _w;         // mm
        private readonly double _h;         // mm
        private readonly bool _showText;

        public LabelCanvas(string display, string raw, double w, double h, bool showText)
        {
            _display = display;
            _raw = raw;
            _w = w;
            _h = h;
            _showText = showText;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(18, 18, 28);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // ── 라벨 캔버스 크기 계산 ─────────────────────────────────────
            float aspect = (float)(_w / _h);
            int canvasW = this.Width - 32;
            int canvasH = this.Height - 20;
            int labelW, labelH;
            if (aspect > (float)canvasW / canvasH)
            {
                labelW = canvasW;
                labelH = (int)(canvasW / aspect);
            }
            else
            {
                labelH = canvasH;
                labelW = (int)(canvasH * aspect);
            }

            int ox = (this.Width - labelW) / 2;
            int oy = (this.Height - labelH) / 2;

            // 그림자
            using var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
            g.FillRectangle(shadowBrush, ox + 4, oy + 4, labelW, labelH);

            // 흰색 라벨 배경
            g.FillRectangle(Brushes.White, ox, oy, labelW, labelH);
            using var borderPen = new Pen(Color.FromArgb(80, 100, 140), 1.5f);
            g.DrawRectangle(borderPen, ox, oy, labelW, labelH);

            using var blackBrush = new SolidBrush(Color.Black);

            // ZPL 좌표계를 화면 좌표계로 변환하는 비율
            // 예시 ZPL 기준 라벨 크기를 dots로 환산 (300 DPI, 예시 라벨 ≈ 4inch×2inch = 1200×600 dots)
            // 실제 라벨 dots
            double labelDotsW = _w / 25.4 * 300.0;
            double labelDotsH = _h / 25.4 * 300.0;
            float scaleX = (float)(labelW / labelDotsW);
            float scaleY = (float)(labelH / labelDotsH);

            if (_showText)
            {
                // ── 체크 ON: 바코드 + 포맷 텍스트 출력 ──────────────────────

                // 1) 바코드 영역 (ZPL: ^FO200,60 ^BY4 ^BCN,100,N,N,N)
                float bcX = ox + 200 * scaleX;
                float bcY = oy + 60 * scaleY;
                float bcW = labelW * 0.75f;
                float bcH = 100 * scaleY;
                DrawBarcodeVisual(g, (int)bcX, (int)bcY, (int)bcW, (int)bcH);

                // 2) 포맷 텍스트 (ZPL: ^FO360,180 ^A0,50,50)
                // Y=180 > Y=60 이므로 바코드보다 아래쪽
                float txtFontSize = Math.Max(7f, 50 * scaleY * 0.72f);
                using var txtFont = new Font("Consolas", txtFontSize, FontStyle.Bold);
                var txtSize = g.MeasureString(_display, txtFont);
                float txtX = ox + 360 * scaleX;
                float txtY = oy + 180 * scaleY;
                if (txtX + txtSize.Width > ox + labelW)
                    txtX = ox + (labelW - txtSize.Width) / 2;
                g.DrawString(_display, txtFont, blackBrush, txtX, txtY);
            }
            else
            {
                // ── 체크 OFF: 인코딩 전용 안내 표시 ─────────────────────────
                using var infoFont = new Font("Segoe UI", Math.Max(8f, labelH * 0.08f), FontStyle.Bold);
                using var infoBrush = new SolidBrush(Color.FromArgb(180, 140, 100));
                string msg1 = "인코딩 전용";
                string msg2 = "화면 출력 없음";
                var s1 = g.MeasureString(msg1, infoFont);
                var s2 = g.MeasureString(msg2, infoFont);
                g.DrawString(msg1, infoFont, infoBrush, ox + (labelW - s1.Width) / 2, oy + labelH * 0.35f);
                g.DrawString(msg2, infoFont, infoBrush, ox + (labelW - s2.Width) / 2, oy + labelH * 0.55f);
            }

            // ── 치수 표기 ─────────────────────────────────────────────────
            using var dimFont = new Font("Segoe UI", 7.5f);
            using var dimBrush = new SolidBrush(Color.FromArgb(120, 130, 160));
            g.DrawString($"{_w:F1} x {_h:F1} mm", dimFont, dimBrush, ox + 4f, oy + labelH + 4);
        }

        private static void DrawBarcodeVisual(Graphics g, int bx, int by, int bw, int bh)
        {
            using var barBrush = new SolidBrush(Color.Black);
            var rng = new Random(42);
            int x = bx;
            bool black = true;
            while (x < bx + bw)
            {
                int barW = rng.Next(2, 6);
                if (black)
                    g.FillRectangle(barBrush, x, by, barW, bh);
                x += barW;
                black = !black;
            }
        }
    }
}