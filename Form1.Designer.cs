using System;

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

        // ── 프로필 그룹 ───────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpProfile;
        private System.Windows.Forms.ComboBox cmbProfile;
        private System.Windows.Forms.Button btnNewProfile;
        private System.Windows.Forms.Button btnEditProfile;
        private System.Windows.Forms.Button btnDeleteProfile;

        // ── 연결 방식 그룹 ────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpConnect;
        private System.Windows.Forms.RadioButton rdoCom;
        private System.Windows.Forms.RadioButton rdoUsb;
        private System.Windows.Forms.RadioButton rdoEthernet;

        private System.Windows.Forms.Panel pnlCom;
        private System.Windows.Forms.Label lblPortLabel;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.Panel pnlUsb;
        private System.Windows.Forms.Label lblPrinterLabel;
        private System.Windows.Forms.ComboBox cmbPrinter;
        private System.Windows.Forms.Button btnRefreshPrinter;

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

        // ── 라벨 설정 그룹 (읽기 전용 표시) ──────────────────────────────
        private System.Windows.Forms.GroupBox grpLabel;
        private System.Windows.Forms.Label lblLabelSizeValue;

        // ── 옵션 그룹 ─────────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkShowBarcode;
        private System.Windows.Forms.CheckBox chkAutoPrint;
        private System.Windows.Forms.Label lblCopiesLabel;
        private System.Windows.Forms.NumericUpDown nudCopies;

        // ── 스캔 목록 그룹 ────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpBarcodeList;
        private System.Windows.Forms.ListBox lstBarcodes;
        private System.Windows.Forms.Label lblScanCount;
        private System.Windows.Forms.Button btnClearList;

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
            this.Size = new System.Drawing.Size(535, 880);
            this.MinimumSize = new System.Drawing.Size(535, 880);
            this.MaximumSize = new System.Drawing.Size(535, 880);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 28);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5f);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Icon = new System.Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo_blue_without_text.ico"));

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
            lblSubtitle.Text = "COM Serial  ·  USB (RAW)  ·  Ethernet (TCP)  ·  ZPL  ·  멀티 프로필  ·  v2.0";
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
            pnlBody.Size = new System.Drawing.Size(520, 690);

            // ══ GROUP: 라벨 프로필 ════════════════════════════════════════
            // y=10, h=80
            grpProfile = MakeGroup("라벨 프로필", 16, 10, 488, 80);

            var lblProfileLabel = MakeLabel("프로필:", 12, 28);

            cmbProfile = new System.Windows.Forms.ComboBox();
            cmbProfile.Location = new System.Drawing.Point(72, 24);
            cmbProfile.Size = new System.Drawing.Size(246, 28);
            cmbProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbProfile.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            cmbProfile.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            cmbProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cmbProfile.Font = new System.Drawing.Font("Segoe UI", 10f);
            cmbProfile.SelectedIndexChanged += new System.EventHandler(this.cmbProfile_SelectedIndexChanged);

            btnNewProfile = MakeButton("➕ 새 프로필", 328, 23, 112, 30);
            btnNewProfile.Click += new System.EventHandler(this.btnNewProfile_Click);

            btnEditProfile = MakeButton("✏ 편집", 72, 56, 72, 22);
            btnEditProfile.Click += new System.EventHandler(this.btnEditProfile_Click);

            btnDeleteProfile = MakeButton("🗑 삭제", 152, 56, 72, 22);
            btnDeleteProfile.ForeColor = System.Drawing.Color.FromArgb(255, 140, 120);
            btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);

            grpProfile.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblProfileLabel, cmbProfile, btnNewProfile, btnEditProfile, btnDeleteProfile
            });

            // ══ GROUP: 연결 방식 ══════════════════════════════════════════
            // y=100, h=140
            grpConnect = MakeGroup("연결 방식", 16, 100, 488, 140);

            rdoCom = MakeRadio("COM 시리얼", 16, 26);
            rdoCom.Checked = true;
            rdoCom.CheckedChanged += new System.EventHandler(this.rdoConnect_CheckedChanged);

            rdoUsb = MakeRadio("USB", 150, 26);
            rdoUsb.CheckedChanged += new System.EventHandler(this.rdoConnect_CheckedChanged);

            rdoEthernet = MakeRadio("Ethernet (TCP/IP)", 224, 26);
            rdoEthernet.CheckedChanged += new System.EventHandler(this.rdoConnect_CheckedChanged);

            // COM 서브 패널
            pnlCom = new System.Windows.Forms.Panel();
            pnlCom.BackColor = System.Drawing.Color.Transparent;
            pnlCom.Location = new System.Drawing.Point(8, 54);
            pnlCom.Size = new System.Drawing.Size(468, 80);
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
            lblComHint.Text = "9600 bps / 데이터비트 8 / 패리티 없음 / 스톱비트 1";
            lblComHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblComHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblComHint.Location = new System.Drawing.Point(4, 48);
            lblComHint.AutoSize = true;

            pnlCom.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblPortLabel, cmbPort, btnRefresh, lblComHint
            });

            // USB 서브 패널
            pnlUsb = new System.Windows.Forms.Panel();
            pnlUsb.BackColor = System.Drawing.Color.Transparent;
            pnlUsb.Location = new System.Drawing.Point(8, 54);
            pnlUsb.Size = new System.Drawing.Size(468, 80);
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
            lblUsbHint.Text = "Windows 설치 프린터 · ZPL RAW 직접 전송 (winspool.drv)";
            lblUsbHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblUsbHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblUsbHint.Location = new System.Drawing.Point(4, 48);
            lblUsbHint.AutoSize = true;

            pnlUsb.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblPrinterLabel, cmbPrinter, btnRefreshPrinter, lblUsbHint
            });

            // Ethernet 서브 패널
            pnlEthernet = new System.Windows.Forms.Panel();
            pnlEthernet.BackColor = System.Drawing.Color.Transparent;
            pnlEthernet.Location = new System.Drawing.Point(8, 54);
            pnlEthernet.Size = new System.Drawing.Size(468, 80);
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
            lblEthHint.Text = "기본 포트: 9100 · TCP 소켓 · 연결 타임아웃 5초";
            lblEthHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblEthHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblEthHint.Location = new System.Drawing.Point(4, 48);
            lblEthHint.AutoSize = true;

            pnlEthernet.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblIpLabel, txtIpAddress, lblEthPortLabel, txtEthPort, lblEthHint
            });

            grpConnect.Controls.AddRange(new System.Windows.Forms.Control[] {
                rdoCom, rdoUsb, rdoEthernet,
                pnlCom, pnlUsb, pnlEthernet
            });

            // ══ GROUP: 바코드 입력 ════════════════════════════════════════
            // y=250, h=88  (타이틀은 ApplyProfileToUI에서 동적 갱신)
            grpBarcode = MakeGroup("바코드 입력", 16, 250, 488, 88);

            lblBarcodeLabel = MakeLabel("바코드:", 12, 28);

            txtBarcode = new System.Windows.Forms.TextBox();
            txtBarcode.Location = new System.Drawing.Point(90, 24);
            txtBarcode.Size = new System.Drawing.Size(260, 28);
            txtBarcode.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            txtBarcode.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            txtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtBarcode.Font = new System.Drawing.Font("Consolas", 13f, System.Drawing.FontStyle.Bold);
            txtBarcode.MaxLength = 20;   // ApplyProfileToUI에서 프로필 값으로 교체
            txtBarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBarcode_KeyPress);

            lblBarcodeHint = new System.Windows.Forms.Label();
            lblBarcodeHint.Text = "";   // ApplyProfileToUI에서 갱신
            lblBarcodeHint.Font = new System.Drawing.Font("Segoe UI", 8f);
            lblBarcodeHint.ForeColor = System.Drawing.Color.FromArgb(120, 130, 160);
            lblBarcodeHint.Location = new System.Drawing.Point(12, 60);
            lblBarcodeHint.AutoSize = true;

            grpBarcode.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblBarcodeLabel, txtBarcode, lblBarcodeHint
            });

            // ══ GROUP: 라벨 설정 (읽기 전용) ══════════════════════════════
            // y=348, h=62
            grpLabel = MakeGroup("라벨 설정", 16, 348, 488, 62);

            var lblLabelSizeTitle = MakeLabel("크기 / DPI:", 12, 24);

            lblLabelSizeValue = new System.Windows.Forms.Label();
            lblLabelSizeValue.Text = "—";
            lblLabelSizeValue.Font = new System.Drawing.Font("Consolas", 10f, System.Drawing.FontStyle.Bold);
            lblLabelSizeValue.ForeColor = System.Drawing.Color.FromArgb(255, 200, 80);
            lblLabelSizeValue.Location = new System.Drawing.Point(110, 22);
            lblLabelSizeValue.AutoSize = true;

            grpLabel.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblLabelSizeTitle, lblLabelSizeValue
            });

            // ══ GROUP: 인쇄 옵션 ══════════════════════════════════════════
            // y=420, h=100
            grpOptions = MakeGroup("인쇄 옵션", 16, 420, 488, 108);

            chkShowBarcode = new System.Windows.Forms.CheckBox();
            chkShowBarcode.Text = "바코드 인쇄 (Human Readable)";
            chkShowBarcode.Font = new System.Drawing.Font("Segoe UI", 10f);
            chkShowBarcode.ForeColor = System.Drawing.Color.FromArgb(200, 210, 230);
            chkShowBarcode.Location = new System.Drawing.Point(16, 24);
            chkShowBarcode.AutoSize = true;
            chkShowBarcode.Checked = true;
            chkShowBarcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            chkAutoPrint = new System.Windows.Forms.CheckBox();
            chkAutoPrint.Text = "스캔 즉시 발행";
            chkAutoPrint.Font = new System.Drawing.Font("Segoe UI", 10f);
            chkAutoPrint.ForeColor = System.Drawing.Color.FromArgb(200, 210, 230);
            chkAutoPrint.Location = new System.Drawing.Point(16, 50);
            chkAutoPrint.AutoSize = true;
            chkAutoPrint.Checked = false;
            chkAutoPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            lblCopiesLabel = MakeLabel("발행 매수:", 16, 78);

            nudCopies = new System.Windows.Forms.NumericUpDown();
            nudCopies.Location = new System.Drawing.Point(90, 74);
            nudCopies.Size = new System.Drawing.Size(68, 28);
            nudCopies.Minimum = 1;
            nudCopies.Maximum = 99;
            nudCopies.Value = 1;
            nudCopies.BackColor = System.Drawing.Color.FromArgb(30, 30, 50);
            nudCopies.ForeColor = System.Drawing.Color.FromArgb(255, 200, 80);
            nudCopies.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            nudCopies.Font = new System.Drawing.Font("Consolas", 11f);

            grpOptions.Controls.AddRange(new System.Windows.Forms.Control[] {
                chkShowBarcode, chkAutoPrint, lblCopiesLabel, nudCopies
            });

            // ══ GROUP: 스캔된 바코드 목록 ══════════════════════════════════
            // y=530, h=160
            grpBarcodeList = MakeGroup("스캔된 바코드 목록  (자동 발행 OFF 시 사용)", 16, 530, 488, 160);

            var lblScanCountTitle = MakeLabel("스캔된 바코드 수:", 12, 26);

            lblScanCount = new System.Windows.Forms.Label();
            lblScanCount.Text = "0 개";
            lblScanCount.Font = new System.Drawing.Font("Consolas", 11f, System.Drawing.FontStyle.Bold);
            lblScanCount.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            lblScanCount.Location = new System.Drawing.Point(158, 24);
            lblScanCount.AutoSize = true;

            btnClearList = MakeButton("✕  목록 초기화", 356, 20, 116, 30);
            btnClearList.ForeColor = System.Drawing.Color.FromArgb(255, 140, 120);
            btnClearList.Click += new System.EventHandler(this.btnClearList_Click);

            lstBarcodes = new System.Windows.Forms.ListBox();
            lstBarcodes.Location = new System.Drawing.Point(12, 56);
            lstBarcodes.Size = new System.Drawing.Size(460, 96);
            lstBarcodes.BackColor = System.Drawing.Color.FromArgb(20, 20, 36);
            lstBarcodes.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            lstBarcodes.Font = new System.Drawing.Font("Consolas", 10f);
            lstBarcodes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lstBarcodes.SelectionMode = System.Windows.Forms.SelectionMode.None;
            lstBarcodes.IntegralHeight = false;

            grpBarcodeList.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblScanCountTitle, lblScanCount, btnClearList, lstBarcodes
            });

            pnlBody.Controls.AddRange(new System.Windows.Forms.Control[] {
                grpProfile, grpConnect, grpBarcode, grpLabel, grpOptions, grpBarcodeList
            });

            // ── Footer ────────────────────────────────────────────────────
            pnlFooter = new System.Windows.Forms.Panel();
            pnlFooter.BackColor = System.Drawing.Color.FromArgb(22, 22, 36);
            pnlFooter.Location = new System.Drawing.Point(0, 770);
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