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
        private readonly LabelProfile _profile;
        private readonly bool _showBarcode;

        public PrintPreviewForm(string zpl, string barcodeDisplay, string barcodeRaw,
            LabelProfile profile, bool showBarcode)
        {
            _zpl = zpl;
            _barcodeDisplay = barcodeDisplay;
            _barcodeRaw = barcodeRaw;
            _profile = profile;
            _showBarcode = showBarcode;
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
            pnlHeader.Controls.Add(new Label
            {
                Text = "인쇄 전 내용을 확인해주세요",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 210, 255),
                Location = new Point(20, 14),
                AutoSize = true
            });

            // ── Info Table ────────────────────────────────────────────────
            var pnlInfo = new Panel
            {
                BackColor = Color.FromArgb(26, 26, 42),
                Location = new Point(16, 68),
                Size = new Size(608, 140),
                Padding = new Padding(12)
            };

            string labelSpec =
                $"{_profile.LabelWidthMm:F1} × {_profile.LabelHeightMm:F1} mm  " +
                $"({_profile.Dpi} DPI)  |  {_profile.Name}";

            AddInfoRow(pnlInfo, "바코드 (RAW):", _barcodeRaw, Color.FromArgb(100, 210, 255), 0);
            AddInfoRow(pnlInfo, "출력 표기:", _barcodeDisplay, Color.FromArgb(100, 210, 255), 34);
            AddInfoRow(pnlInfo, "라벨 설정:", labelSpec, Color.FromArgb(255, 200, 80), 68);
            AddInfoRow(pnlInfo, "출력 모드:",
                _showBarcode ? "바코드 + 텍스트 출력" : "인코딩 전용 (화면 출력 없음)",
                _showBarcode ? Color.FromArgb(0, 200, 120) : Color.FromArgb(255, 160, 40), 102);

            // ── Label canvas ──────────────────────────────────────────────
            // pnlInfo 하단(68+140=208) + 마진
            var lblPreviewTitle = new Label
            {
                Text = "라벨 미리보기 (시각적 참고용)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 130, 160),
                Location = new Point(16, 220),
                AutoSize = true
            };

            var canvas = new LabelCanvas(_barcodeDisplay, _barcodeRaw,
                _profile.LabelWidthMm, _profile.LabelHeightMm, _showBarcode)
            {
                Location = new Point(16, 242),
                Size = new Size(608, 240)
            };

            // canvas 하단(242+240=482) + 여백
            var btnToggle = new Button
            {
                Text = "▾  ZPL 커멘드 보기",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 44, 60),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(16, 494),
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
                Location = new Point(16, 494),
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
            panel.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(140, 150, 170),
                Location = new Point(12, y + 8),
                Size = new Size(140, 24)
            });
            panel.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Consolas", 9.5f, FontStyle.Bold),
                ForeColor = valueColor,
                Location = new Point(155, y + 8),
                AutoSize = true
            });
        }
    }

    /// <summary>
    /// 라벨을 시각적으로 그려주는 커스텀 컨트롤
    /// </summary>
    public class LabelCanvas : Control
    {
        private readonly string _display;
        private readonly string _raw;
        private readonly double _w;
        private readonly double _h;
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

            // 캔버스 내에서 비율 유지
            float aspect = (float)(_w / _h);
            int cw = this.Width - 32, ch = this.Height - 20;
            int labelW, labelH;
            if (aspect > (float)cw / ch) { labelW = cw; labelH = (int)(cw / aspect); }
            else { labelH = ch; labelW = (int)(ch * aspect); }

            int ox = (this.Width - labelW) / 2;
            int oy = (this.Height - labelH) / 2;

            // 그림자
            using var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
            g.FillRectangle(shadow, ox + 4, oy + 4, labelW, labelH);

            // 흰색 라벨
            g.FillRectangle(Brushes.White, ox, oy, labelW, labelH);
            using var border = new Pen(Color.FromArgb(80, 100, 140), 1.5f);
            g.DrawRectangle(border, ox, oy, labelW, labelH);

            // 300 DPI 기준 스케일
            double dotsW = _w / 25.4 * 300.0;
            double dotsH = _h / 25.4 * 300.0;
            float sx = (float)(labelW / dotsW);
            float sy = (float)(labelH / dotsH);

            using var blackBrush = new SolidBrush(Color.Black);

            if (_showText)
            {
                // 바코드 영역 (ZPL 기본: ^FO200,60 ^BY4 ^BCN,100)
                float bcX = ox + 200 * sx;
                float bcY = oy + 60 * sy;
                int bcW = (int)(labelW * 0.75f);
                int bcH = (int)(100 * sy);
                DrawBarcodeStripes(g, (int)bcX, (int)bcY, bcW, bcH);

                // 텍스트 (ZPL 기본: ^FO360,180 ^A0,50,50)
                float fontSize = Math.Max(7f, 50 * sy * 0.72f);
                using var font = new Font("Consolas", fontSize, FontStyle.Bold);
                var sz = g.MeasureString(_display, font);
                float tx = ox + 360 * sx;
                float ty = oy + 180 * sy;
                if (tx + sz.Width > ox + labelW) tx = ox + (labelW - sz.Width) / 2;
                g.DrawString(_display, font, blackBrush, tx, ty);
            }
            else
            {
                using var infoFont = new Font("Segoe UI", Math.Max(8f, labelH * 0.08f), FontStyle.Bold);
                using var infoBrush = new SolidBrush(Color.FromArgb(180, 140, 100));
                foreach ((string msg, float yFrac) in new[] { ("인코딩 전용", 0.35f), ("화면 출력 없음", 0.55f) })
                {
                    var s = g.MeasureString(msg, infoFont);
                    g.DrawString(msg, infoFont, infoBrush,
                        ox + (labelW - s.Width) / 2, oy + labelH * yFrac);
                }
            }

            // 치수 표기
            using var dimFont = new Font("Segoe UI", 7.5f);
            using var dimBrush = new SolidBrush(Color.FromArgb(120, 130, 160));
            g.DrawString($"{_w:F1} × {_h:F1} mm", dimFont, dimBrush, ox + 4f, oy + labelH + 4);
        }

        private static void DrawBarcodeStripes(Graphics g, int bx, int by, int bw, int bh)
        {
            using var brush = new SolidBrush(Color.Black);
            var rng = new Random(42);
            int x = bx;
            bool black = true;
            while (x < bx + bw)
            {
                int w = rng.Next(2, 6);
                if (black) g.FillRectangle(brush, x, by, w, bh);
                x += w;
                black = !black;
            }
        }
    }
}