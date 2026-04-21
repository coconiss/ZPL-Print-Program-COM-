using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ZplPrinter
{
    /// <summary>
    /// 라벨 프로필을 생성하거나 편집하는 모달 다이얼로그.
    /// DialogResult.OK 로 닫힐 때 전달된 profile 객체에 변경이 반영된다.
    /// </summary>
    public class ProfileEditorForm : Form
    {
        private readonly LabelProfile _profile;

        // Tab 1 컨트롤
        private TextBox _txtName = null!;
        private TextBox _txtDescription = null!;
        private NumericUpDown _nudWidth = null!;
        private NumericUpDown _nudHeight = null!;
        private NumericUpDown _nudDpi = null!;
        private NumericUpDown _nudInputLen = null!;
        private TextBox _txtSegments = null!;
        private NumericUpDown _nudDelay = null!;

        // Tab 2 컨트롤
        private TextBox _txtZplWithBarcode = null!;
        private TextBox _txtZplEncodingOnly = null!;

        public ProfileEditorForm(LabelProfile profile, bool isNew)
        {
            _profile = profile;
            Build(isNew);
            LoadValues();
        }

        // ── UI 생성 ─────────────────────────────────────────────────────

        private void Build(bool isNew)
        {
            Text = isNew ? "새 프로필 추가" : "프로필 편집";
            Size = new Size(700, 620);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(18, 18, 28);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Header
            var pnlHead = new Panel
            {
                BackColor = Color.FromArgb(26, 26, 42),
                Dock = DockStyle.Top,
                Height = 52
            };
            pnlHead.Controls.Add(new Label
            {
                Text = isNew ? "새 라벨 프로필 만들기" : "라벨 프로필 편집",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 210, 255),
                Location = new Point(20, 13),
                AutoSize = true
            });

            // Tabs
            var tabs = new TabControl
            {
                Location = new Point(12, 62),
                Size = new Size(668, 462),
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(130, 28),
                SizeMode = TabSizeMode.Fixed
            };
            tabs.DrawItem += DrawTabItem;

            var tab1 = new TabPage("기본 설정") { BackColor = Color.FromArgb(22, 22, 36), Padding = new Padding(0) };
            var tab2 = new TabPage("ZPL 템플릿") { BackColor = Color.FromArgb(22, 22, 36), Padding = new Padding(0) };

            BuildTab1(tab1);
            BuildTab2(tab2);
            tabs.TabPages.Add(tab1);
            tabs.TabPages.Add(tab2);

            // Footer buttons
            var pnlFoot = new Panel
            {
                BackColor = Color.FromArgb(22, 22, 36),
                Dock = DockStyle.Bottom,
                Height = 52
            };

            var btnOk = Btn("✓  저장", Color.FromArgb(40, 120, 220), 466, 8, 110, 36);
            btnOk.Click += BtnOk_Click;

            var btnCancel = Btn("✗  취소", Color.FromArgb(60, 40, 40), 590, 8, 88, 36);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            pnlFoot.Controls.AddRange(new Control[] { btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;

            Controls.AddRange(new Control[] { pnlHead, tabs, pnlFoot });
        }

        private void BuildTab1(TabPage tab)
        {
            int y = 14;

            void Row(string lbl, Control ctrl, int ctrlW = 380, string? hint = null)
            {
                tab.Controls.Add(new Label
                {
                    Text = lbl,
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(180, 190, 210),
                    Location = new Point(16, y + 3),
                    Size = new Size(148, 22)
                });
                ctrl.Location = new Point(166, y);
                ctrl.Width = ctrlW;
                tab.Controls.Add(ctrl);
                if (hint != null)
                    tab.Controls.Add(new Label
                    {
                        Text = hint,
                        Font = new Font("Segoe UI", 7.5f),
                        ForeColor = Color.FromArgb(110, 120, 150),
                        Location = new Point(166 + ctrlW + 8, y + 5),
                        AutoSize = true
                    });
                y += 36;
            }

            _txtName = Txt(Color.FromArgb(100, 210, 255));
            Row("프로필 이름:", _txtName);

            _txtDescription = Txt(Color.FromArgb(180, 190, 210));
            Row("설명:", _txtDescription);

            _nudWidth = Nud(1, 9999, 0.5m, 1);
            Row("라벨 가로 (mm):", _nudWidth, 120, "실제 라벨 폭");

            _nudHeight = Nud(1, 9999, 0.5m, 1);
            Row("라벨 세로 (mm):", _nudHeight, 120, "실제 라벨 높이");

            _nudDpi = Nud(72, 1200, 1, 0);
            Row("DPI:", _nudDpi, 120, "프린터 해상도");

            _nudInputLen = Nud(1, 50, 1, 0);
            Row("바코드 입력 길이:", _nudInputLen, 90, "스캐너 입력 자릿수");

            _txtSegments = Txt(Color.FromArgb(255, 200, 80));
            Row("세그먼트 구분:", _txtSegments, 180, "예: 3,2,7  →  AAA-BB-CCCCCCC");

            _nudDelay = Nud(0, 30000, 100, 0);
            Row("인쇄 간격 (ms):", _nudDelay, 120, "연속 인쇄 시 딜레이");
        }

        private void BuildTab2(TabPage tab)
        {
            var lblHelp = new Label
            {
                Text = "토큰:   {BARCODE}   {BARCODE_DISPLAY}   {COPIES}",
                Font = new Font("Consolas", 9f),
                ForeColor = Color.FromArgb(120, 220, 100),
                Location = new Point(12, 10),
                AutoSize = true
            };
            tab.Controls.Add(lblHelp);

            tab.Controls.Add(Lbl("▸ 바코드 출력 모드  (체크 ON)", 12, 36));
            _txtZplWithBarcode = ZplBox(new Point(12, 58), new Size(636, 140));
            tab.Controls.Add(_txtZplWithBarcode);

            tab.Controls.Add(Lbl("▸ 인코딩 전용 모드  (체크 OFF)", 12, 210));
            _txtZplEncodingOnly = ZplBox(new Point(12, 232), new Size(636, 140));
            tab.Controls.Add(_txtZplEncodingOnly);

            var btnReset = Btn("↺  기본값 복원", Color.FromArgb(50, 60, 90), 12, 386, 140, 28);
            btnReset.Click += (s, e) =>
            {
                _txtZplWithBarcode.Text = LabelProfile.DefaultZplWithBarcode;
                _txtZplEncodingOnly.Text = LabelProfile.DefaultZplEncodingOnly;
            };
            tab.Controls.Add(btnReset);
        }

        // ── 값 로드 / 저장 ─────────────────────────────────────────────

        private void LoadValues()
        {
            _txtName.Text = _profile.Name;
            _txtDescription.Text = _profile.Description;
            _nudWidth.Value = (decimal)_profile.LabelWidthMm;
            _nudHeight.Value = (decimal)_profile.LabelHeightMm;
            _nudDpi.Value = _profile.Dpi;
            _nudInputLen.Value = _profile.BarcodeInputLength;
            _txtSegments.Text = _profile.SegmentsToString();
            _nudDelay.Value = _profile.PrintDelayMs;
            _txtZplWithBarcode.Text = _profile.ZplTemplate;
            _txtZplEncodingOnly.Text = _profile.ZplTemplateEncodingOnly;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("프로필 이름을 입력해주세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _profile.Name = _txtName.Text.Trim();
            _profile.Description = _txtDescription.Text.Trim();
            _profile.LabelWidthMm = (double)_nudWidth.Value;
            _profile.LabelHeightMm = (double)_nudHeight.Value;
            _profile.Dpi = (int)_nudDpi.Value;
            _profile.BarcodeInputLength = (int)_nudInputLen.Value;
            _profile.SetSegmentsFromString(_txtSegments.Text);
            _profile.PrintDelayMs = (int)_nudDelay.Value;
            _profile.ZplTemplate = _txtZplWithBarcode.Text;
            _profile.ZplTemplateEncodingOnly = _txtZplEncodingOnly.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        // ── 탭 커스텀 드로우 ──────────────────────────────────────────

        private static void DrawTabItem(object? sender, DrawItemEventArgs e)
        {
            if (sender is not TabControl tc) return;
            bool sel = e.Index == tc.SelectedIndex;
            using var bg = new SolidBrush(sel
                ? Color.FromArgb(40, 120, 220)
                : Color.FromArgb(30, 30, 50));
            e.Graphics.FillRectangle(bg, e.Bounds);
            TextRenderer.DrawText(e.Graphics, tc.TabPages[e.Index].Text,
                new Font("Segoe UI", 9f, FontStyle.Bold), e.Bounds,
                sel ? Color.White : Color.FromArgb(140, 150, 170),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ── 헬퍼 팩토리 ───────────────────────────────────────────────

        private static TextBox Txt(Color fg) => new TextBox
        {
            BackColor = Color.FromArgb(30, 30, 50),
            ForeColor = fg,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10f)
        };

        private static NumericUpDown Nud(decimal min, decimal max, decimal inc, int decimals) =>
            new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Increment = inc,
                DecimalPlaces = decimals,
                BackColor = Color.FromArgb(30, 30, 50),
                ForeColor = Color.FromArgb(255, 200, 80),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10f)
            };

        private static Button Btn(string text, Color bg, int x, int y, int w, int h)
        {
            var b = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = bg,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private static Label Lbl(string text, int x, int y) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 210, 255),
            Location = new Point(x, y),
            AutoSize = true
        };

        private static TextBox ZplBox(Point loc, Size sz) => new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(14, 14, 24),
            ForeColor = Color.FromArgb(120, 220, 100),
            Font = new Font("Consolas", 9f),
            BorderStyle = BorderStyle.FixedSingle,
            Location = loc,
            Size = sz
        };
    }
}