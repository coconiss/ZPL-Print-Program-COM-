namespace ZplPrinter
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // ── 공통 레이아웃 ─────────────────────────────────────────────────
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label lblStatus;

        // ── 연결 방식 그룹 ────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpConnect;
        private System.Windows.Forms.RadioButton rdoCom;
        private System.Windows.Forms.RadioButton rdoUsb;
        private System.Windows.Forms.RadioButton rdoEthernet;

        // COM 서브 패널
        private System.Windows.Forms.Panel pnlCom;
        private System.Windows.Forms.Label lblPortLabel;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Button btnRefresh;

        // USB 서브 패널
        private System.Windows.Forms.Panel pnlUsb;
        private System.Windows.Forms.Label lblPrinterLabel;
        private System.Windows.Forms.ComboBox cmbPrinter;
        private System.Windows.Forms.Button btnRefreshPrinter;

        // Ethernet 서브 패널
        private System.Windows.Forms.Panel pnlEthernet;
        private System.Windows.Forms.Label lblIpLabel;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label lblEthPortLabel;
        private System.Windows.Forms.TextBox txtEthPort;

        // ── 바코드 그룹 ───────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpBarcode;
        private System.Windows.Forms.Label lblBarcodeLabel;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblBarcodeHint;

        // ── 라벨 크기 그룹 ────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpLabel;
        private System.Windows.Forms.Label lblWidthLabel;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label lblWidthUnit;
        private System.Windows.Forms.Label lblHeightLabel;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label lblHeightUnit;
        private System.Windows.Forms.Label lblDpiLabel;
        private System.Windows.Forms.Label lblDpiValue;

        // ── 옵션 그룹 ─────────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkShowBarcode;

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
            lblSubtitle.Text = "COM Serial  ·  USB (RAW)  ·  Ethernet (TCP)  ·  ZPL  ·  300 DPI";
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

            // ══ GROUP: 연결 방식 ══════════════════════════════════════════
            grpConnect = MakeGroup("연결 방식", 16, 10, 488, 160);

            // 라디오 버튼 (이벤트 등록 전에 Checked 설정)
            rdoCom = MakeRadio("COM 시리얼", 16, 26);
            rdoCom.Checked = true;
            rdoCom.CheckedChanged += new System.EventHandler(this.rdoConnect_CheckedChanged);

            rdoUsb = MakeRadio("USB", 150, 26);
            rdoUsb.CheckedChanged += new System.EventHandler(this.rdoConnect_CheckedChanged);

            rdoEthernet = MakeRadio("Ethernet (TCP/IP)", 224, 26);
            rdoEthernet.CheckedChanged += new System.EventHandler(this.rdoConnect_CheckedChanged);

            // ── COM 서브 패널 ─────────────────────────────────────────────
            pnlCom = new System.Windows.Forms.Panel();
            pnlCom.BackColor = System.Drawing.Color.Transparent;
            pnlCom.Location = new System.Drawing.Point(8, 54);
            pnlCom.Size = new System.Drawing.Size(468, 90);
            pnlCom.Visible = true;

            lblPortLabel = MakeLabel("포트:", 4, 14);

            cmbPort = new System.Windows.Forms.ComboBox();
            cmbPort.Location = new System.Drawing.Point(62, 10);
            cmbPort.Size = new System.Drawing.Size(270, 28);
            cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPort.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            cmbPort.ForeColor = System.Drawing.Color.White;
            cmbPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cmbPort.Font = new System.Drawing.Font("Segoe UI", 10f);

            btnRefresh = MakeButton("↻ 새로고침", 344, 9, 100, 30);
            btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            var lblComHint = new System.Windows.Forms.Label();
            lblComHint.Text = "통신 파라미터: 9600 bps / 데이터비트 8 / 패리티 없음 / 스톱비트 1";
            lblComHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblComHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblComHint.Location = new System.Drawing.Point(4, 50);
            lblComHint.AutoSize = true;

            pnlCom.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblPortLabel, cmbPort, btnRefresh, lblComHint
            });

            // ── USB 서브 패널 ─────────────────────────────────────────────
            pnlUsb = new System.Windows.Forms.Panel();
            pnlUsb.BackColor = System.Drawing.Color.Transparent;
            pnlUsb.Location = new System.Drawing.Point(8, 54);
            pnlUsb.Size = new System.Drawing.Size(468, 90);
            pnlUsb.Visible = false;

            lblPrinterLabel = MakeLabel("프린터:", 4, 14);

            cmbPrinter = new System.Windows.Forms.ComboBox();
            cmbPrinter.Location = new System.Drawing.Point(72, 10);
            cmbPrinter.Size = new System.Drawing.Size(260, 28);
            cmbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPrinter.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            cmbPrinter.ForeColor = System.Drawing.Color.White;
            cmbPrinter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cmbPrinter.Font = new System.Drawing.Font("Segoe UI", 9.5f);

            btnRefreshPrinter = MakeButton("↻ 새로고침", 344, 9, 100, 30);
            btnRefreshPrinter.Click += new System.EventHandler(this.btnRefreshPrinter_Click);

            var lblUsbHint = new System.Windows.Forms.Label();
            lblUsbHint.Text = "Windows 설치 프린터 목록 · ZPL RAW 데이터 직접 전송 (winspool.drv)";
            lblUsbHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblUsbHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblUsbHint.Location = new System.Drawing.Point(4, 50);
            lblUsbHint.AutoSize = true;

            pnlUsb.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblPrinterLabel, cmbPrinter, btnRefreshPrinter, lblUsbHint
            });

            // ── Ethernet 서브 패널 ────────────────────────────────────────
            pnlEthernet = new System.Windows.Forms.Panel();
            pnlEthernet.BackColor = System.Drawing.Color.Transparent;
            pnlEthernet.Location = new System.Drawing.Point(8, 54);
            pnlEthernet.Size = new System.Drawing.Size(468, 90);
            pnlEthernet.Visible = false;

            lblIpLabel = MakeLabel("IP 주소:", 4, 14);

            txtIpAddress = new System.Windows.Forms.TextBox();
            txtIpAddress.Location = new System.Drawing.Point(78, 10);
            txtIpAddress.Size = new System.Drawing.Size(160, 28);
            txtIpAddress.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            txtIpAddress.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            txtIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtIpAddress.Font = new System.Drawing.Font("Consolas", 11f);
            txtIpAddress.Text = "192.168.1.100";

            lblEthPortLabel = MakeLabel("포트:", 252, 14);

            txtEthPort = new System.Windows.Forms.TextBox();
            txtEthPort.Location = new System.Drawing.Point(296, 10);
            txtEthPort.Size = new System.Drawing.Size(76, 28);
            txtEthPort.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            txtEthPort.ForeColor = System.Drawing.Color.FromArgb(255, 200, 80);
            txtEthPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtEthPort.Font = new System.Drawing.Font("Consolas", 11f);
            txtEthPort.Text = "9100";

            var lblEthHint = new System.Windows.Forms.Label();
            lblEthHint.Text = "기본 포트: 9100 (Zebra ZPL 표준) · TCP 소켓 전송 · 연결 타임아웃 5초";
            lblEthHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblEthHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblEthHint.Location = new System.Drawing.Point(4, 50);
            lblEthHint.AutoSize = true;

            pnlEthernet.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblIpLabel, txtIpAddress, lblEthPortLabel, txtEthPort, lblEthHint
            });

            grpConnect.Controls.AddRange(new System.Windows.Forms.Control[] {
                rdoCom, rdoUsb, rdoEthernet,
                pnlCom, pnlUsb, pnlEthernet
            });

            // ══ GROUP: 바코드 입력 ════════════════════════════════════════
            grpBarcode = MakeGroup("바코드 입력 (12자리 숫자)", 16, 185, 488, 100);

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

            grpBarcode.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblBarcodeLabel, txtBarcode, lblBarcodeHint
            });

            // ══ GROUP: 라벨 크기 ══════════════════════════════════════════
            grpLabel = MakeGroup("라벨 크기 (단위: mm)", 16, 300, 488, 110);

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

            // ══ GROUP: 인쇄 옵션 ══════════════════════════════════════════
            grpOptions = MakeGroup("인쇄 옵션", 16, 425, 488, 70);

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
                grpConnect, grpBarcode, grpLabel, grpOptions
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
        private static System.Windows.Forms.RadioButton MakeRadio(string text, int x, int y)
        {
            var r = new System.Windows.Forms.RadioButton();
            r.Text = text;
            r.Font = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            r.ForeColor = System.Drawing.Color.FromArgb(200, 210, 230);
            r.Location = new System.Drawing.Point(x, y);
            r.AutoSize = true;
            r.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            r.Cursor = System.Windows.Forms.Cursors.Hand;
            return r;
        }

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