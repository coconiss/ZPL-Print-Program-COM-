namespace ZplPrinter
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlBody;

        private System.Windows.Forms.GroupBox grpPort;
        private System.Windows.Forms.Label lblPortLabel;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.GroupBox grpBarcode;
        private System.Windows.Forms.Label lblBarcodeLabel;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblBarcodeHint;

        private System.Windows.Forms.GroupBox grpLabel;
        private System.Windows.Forms.Label lblWidthLabel;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label lblWidthUnit;
        private System.Windows.Forms.Label lblHeightLabel;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label lblHeightUnit;
        private System.Windows.Forms.Label lblDpiLabel;
        private System.Windows.Forms.Label lblDpiValue;

        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkShowBarcode;

        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────────
            this.Text = "ZPL Barcode Printer";
            this.Size = new System.Drawing.Size(520, 680);
            this.MinimumSize = new System.Drawing.Size(520, 680);
            this.MaximumSize = new System.Drawing.Size(520, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 28);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5f);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── Header ────────────────────────────────────────────────────
            pnlHeader = new System.Windows.Forms.Panel();
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(26, 26, 42);
            pnlHeader.Location = new System.Drawing.Point(0, 0);
            pnlHeader.Size = new System.Drawing.Size(520, 80);

            lblTitle = new System.Windows.Forms.Label();
            lblTitle.Text = "ZPL BARCODE PRINTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16f, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            lblTitle.Location = new System.Drawing.Point(24, 12);
            lblTitle.AutoSize = true;

            lblSubtitle = new System.Windows.Forms.Label();
            lblSubtitle.Text = "COM Serial Port  ·  ZPL Command  ·  300 DPI";
            lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblSubtitle.Location = new System.Drawing.Point(26, 44);
            lblSubtitle.AutoSize = true;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // ── Body ──────────────────────────────────────────────────────
            pnlBody = new System.Windows.Forms.Panel();
            pnlBody.BackColor = System.Drawing.Color.Transparent;
            pnlBody.Location = new System.Drawing.Point(0, 80);
            pnlBody.Size = new System.Drawing.Size(520, 500);
            pnlBody.Padding = new System.Windows.Forms.Padding(16);

            // GROUP: COM Port
            grpPort = MakeGroup("COM 포트 선택", 16, 10, 488, 80);

            lblPortLabel = MakeLabel("포트:", 12, 28);
            cmbPort = new System.Windows.Forms.ComboBox();
            cmbPort.Location = new System.Drawing.Point(70, 24);
            cmbPort.Size = new System.Drawing.Size(300, 28);
            cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPort.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            cmbPort.ForeColor = System.Drawing.Color.White;
            cmbPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cmbPort.Font = new System.Drawing.Font("Segoe UI", 10f);

            btnRefresh = MakeButton("↻ 새로고침", 382, 23, 90, 30);
            btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            grpPort.Controls.AddRange(new System.Windows.Forms.Control[] { lblPortLabel, cmbPort, btnRefresh });

            // GROUP: Barcode
            grpBarcode = MakeGroup("바코드 입력 (12자리 숫자)", 16, 105, 488, 100);

            lblBarcodeLabel = MakeLabel("바코드:", 12, 28);
            txtBarcode = new System.Windows.Forms.TextBox();
            txtBarcode.Location = new System.Drawing.Point(90, 24);
            txtBarcode.Size = new System.Drawing.Size(260, 28);
            txtBarcode.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            txtBarcode.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            txtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtBarcode.Font = new System.Drawing.Font("Consolas", 13f, System.Drawing.FontStyle.Bold);
            txtBarcode.MaxLength = 12;
            txtBarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBarcode_KeyPress);

            lblBarcodeHint = new System.Windows.Forms.Label();
            lblBarcodeHint.Text = "예) 252190021426  →  출력: 252-19-0021426";
            lblBarcodeHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblBarcodeHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblBarcodeHint.Location = new System.Drawing.Point(12, 62);
            lblBarcodeHint.AutoSize = true;

            grpBarcode.Controls.AddRange(new System.Windows.Forms.Control[] { lblBarcodeLabel, txtBarcode, lblBarcodeHint });

            // GROUP: Label Size
            grpLabel = MakeGroup("라벨 크기 (단위: mm)", 16, 220, 488, 110);

            lblWidthLabel = MakeLabel("가로 (W):", 12, 30);
            txtWidth = MakeNumericTextBox(100, 26, 110, 28);
            txtWidth.Text = "94";
            lblWidthUnit = MakeLabel("mm", 222, 30);

            lblHeightLabel = MakeLabel("세로 (H):", 12, 68);
            txtHeight = MakeNumericTextBox(100, 64, 110, 28);
            txtHeight.Text = "26";
            lblHeightUnit = MakeLabel("mm", 222, 68);

            lblDpiLabel = MakeLabel("DPI:", 300, 30);
            lblDpiValue = new System.Windows.Forms.Label();
            lblDpiValue.Text = "300 (고정)";
            lblDpiValue.Font = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            lblDpiValue.ForeColor = System.Drawing.Color.FromArgb(255, 180, 60);
            lblDpiValue.Location = new System.Drawing.Point(345, 28);
            lblDpiValue.AutoSize = true;

            grpLabel.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblWidthLabel, txtWidth, lblWidthUnit,
                lblHeightLabel, txtHeight, lblHeightUnit,
                lblDpiLabel, lblDpiValue
            });

            // GROUP: Options
            grpOptions = MakeGroup("인쇄 옵션", 16, 345, 488, 70);

            chkShowBarcode = new System.Windows.Forms.CheckBox();
            chkShowBarcode.Text = "바코드 아래 숫자 텍스트 출력 (Human Readable)";
            chkShowBarcode.Font = new System.Drawing.Font("Segoe UI", 10f);
            chkShowBarcode.ForeColor = System.Drawing.Color.FromArgb(200, 210, 230);
            chkShowBarcode.Location = new System.Drawing.Point(16, 28);
            chkShowBarcode.AutoSize = true;
            chkShowBarcode.Checked = true;
            chkShowBarcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            grpOptions.Controls.Add(chkShowBarcode);

            pnlBody.Controls.AddRange(new System.Windows.Forms.Control[] {
                grpPort, grpBarcode, grpLabel, grpOptions
            });

            // ── Footer ────────────────────────────────────────────────────
            pnlFooter = new System.Windows.Forms.Panel();
            pnlFooter.BackColor = System.Drawing.Color.FromArgb(22, 22, 36);
            pnlFooter.Location = new System.Drawing.Point(0, 580);
            pnlFooter.Size = new System.Drawing.Size(520, 70);

            btnPrint = new System.Windows.Forms.Button();
            btnPrint.Text = "▶  인 쇄  전 송";
            btnPrint.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
            btnPrint.ForeColor = System.Drawing.Color.White;
            btnPrint.BackColor = System.Drawing.Color.FromArgb(40, 120, 220);
            btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Location = new System.Drawing.Point(16, 12);
            btnPrint.Size = new System.Drawing.Size(200, 44);
            btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            btnPrint.Click += new System.EventHandler(this.btnPrint_Click);

            lblStatus = new System.Windows.Forms.Label();
            lblStatus.Text = "준비 완료";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 9f);
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblStatus.Location = new System.Drawing.Point(230, 22);
            lblStatus.Size = new System.Drawing.Size(270, 30);

            pnlFooter.Controls.AddRange(new System.Windows.Forms.Control[] { btnPrint, lblStatus });

            // ── Add to Form ───────────────────────────────────────────────
            this.Controls.AddRange(new System.Windows.Forms.Control[] { pnlHeader, pnlBody, pnlFooter });

            this.ResumeLayout(false);
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private static System.Windows.Forms.GroupBox MakeGroup(string title, int x, int y, int w, int h)
        {
            var g = new System.Windows.Forms.GroupBox();
            g.Text = title;
            g.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            g.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            g.BackColor = System.Drawing.Color.FromArgb(26, 26, 42);
            g.Location = new System.Drawing.Point(x, y);
            g.Size = new System.Drawing.Size(w, h);
            return g;
        }

        private static System.Windows.Forms.Label MakeLabel(string text, int x, int y)
        {
            var l = new System.Windows.Forms.Label();
            l.Text = text;
            l.Font = new System.Drawing.Font("Segoe UI", 9.5f);
            l.ForeColor = System.Drawing.Color.FromArgb(180, 190, 210);
            l.Location = new System.Drawing.Point(x, y);
            l.AutoSize = true;
            return l;
        }

        private static System.Windows.Forms.TextBox MakeNumericTextBox(int x, int y, int w, int h)
        {
            var t = new System.Windows.Forms.TextBox();
            t.Location = new System.Drawing.Point(x, y);
            t.Size = new System.Drawing.Size(w, h);
            t.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            t.ForeColor = System.Drawing.Color.FromArgb(255, 200, 80);
            t.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            t.Font = new System.Drawing.Font("Consolas", 11f);
            return t;
        }

        private static System.Windows.Forms.Button MakeButton(string text, int x, int y, int w, int h)
        {
            var b = new System.Windows.Forms.Button();
            b.Text = text;
            b.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            b.ForeColor = System.Drawing.Color.White;
            b.BackColor = System.Drawing.Color.FromArgb(50, 60, 90);
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Location = new System.Drawing.Point(x, y);
            b.Size = new System.Drawing.Size(w, h);
            b.Cursor = System.Windows.Forms.Cursors.Hand;
            return b;
        }
    }
}