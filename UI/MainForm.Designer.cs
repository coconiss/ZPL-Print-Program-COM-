namespace ZplPrinter.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbConnType, cmbCom, cmbUsb, cmbProfile;
        private System.Windows.Forms.ComboBox cmbBaudRate, cmbDataBits, cmbParity, cmbStopBits;
        private System.Windows.Forms.TextBox txtIp, txtPort, txtQuickScan;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Label lblStatus, lblTarget, lblTcpPort, lblComSetup;
        private System.Windows.Forms.Label lblBaud, lblData, lblParity, lblStop;
        private System.Windows.Forms.Button btnPrintSelected, btnPrintAll, btnEditProf, btnNewProf, btnDeleteProf, btnDeleteRow;
        private System.Windows.Forms.CheckBox chkShowBarcode, chkAutoPrint;
        private System.Windows.Forms.NumericUpDown nudCopies;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuOpenSource;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbConnType = new System.Windows.Forms.ComboBox();
            this.cmbCom = new System.Windows.Forms.ComboBox();
            this.cmbUsb = new System.Windows.Forms.ComboBox();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.cmbProfile = new System.Windows.Forms.ComboBox();

            this.btnEditProf = new System.Windows.Forms.Button();
            this.btnNewProf = new System.Windows.Forms.Button();
            this.btnDeleteProf = new System.Windows.Forms.Button();

            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();

            this.txtQuickScan = new System.Windows.Forms.TextBox();
            this.chkShowBarcode = new System.Windows.Forms.CheckBox();
            this.chkAutoPrint = new System.Windows.Forms.CheckBox();
            this.nudCopies = new System.Windows.Forms.NumericUpDown();

            this.dgvData = new System.Windows.Forms.DataGridView();
            this.btnPrintSelected = new System.Windows.Forms.Button();
            this.btnPrintAll = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();

            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblTcpPort = new System.Windows.Forms.Label();
            this.lblComSetup = new System.Windows.Forms.Label();
            this.lblBaud = new System.Windows.Forms.Label();
            this.lblData = new System.Windows.Forms.Label();
            this.lblParity = new System.Windows.Forms.Label();
            this.lblStop = new System.Windows.Forms.Label();

            this.SuspendLayout();

            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenSource = new System.Windows.Forms.ToolStripMenuItem();

            // MenuStrip 설정 (다크 테마 적용)
            this.menuStripMain.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            this.menuStripMain.ForeColor = System.Drawing.Color.White;
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuHelp });
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(920, 24);
            this.menuStripMain.TabIndex = 0;

            // 도움말 메뉴 설정
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuOpenSource });
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(72, 20);
            this.menuHelp.Text = "도움말(&H)";

            // 라이선스-오픈소스 서브메뉴 설정
            this.menuOpenSource.BackColor = System.Drawing.Color.FromArgb(40, 40, 50);
            this.menuOpenSource.ForeColor = System.Drawing.Color.White;
            this.menuOpenSource.Name = "menuOpenSource";
            this.menuOpenSource.Size = new System.Drawing.Size(186, 22);
            this.menuOpenSource.Text = "라이선스 - 오픈소스";
            this.menuOpenSource.Click += new System.EventHandler(this.menuOpenSource_Click);

            // 폼에 MenuStrip 등록 (this.Controls.Add 목록에도 추가되어야 합니다)
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;

            this.Text = "ZPL Universal RFID Printer PRO";
            // ── 폼 가로 사이즈 축소 (1100 -> 920) ──
            this.Size = new System.Drawing.Size(920, 750);
            this.BackColor = System.Drawing.Color.FromArgb(20, 20, 30);
            this.ForeColor = System.Drawing.Color.White;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var inputBg = System.Drawing.Color.FromArgb(50, 50, 60);
            var inputFg = System.Drawing.Color.White;

            // ── 상단 연결 영역 ──
            var pnlTop = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Top, Height = 100, BackColor = System.Drawing.Color.FromArgb(30, 30, 45) };

            pnlTop.Controls.Add(new System.Windows.Forms.Label { Text = "연결 방식:", Location = new System.Drawing.Point(15, 15), AutoSize = true, ForeColor = System.Drawing.Color.LightGray });
            cmbConnType.Items.AddRange(new[] { "COM", "USB", "TCP/IP" });
            cmbConnType.SetBounds(15, 35, 90, 25);
            cmbConnType.BackColor = inputBg; cmbConnType.ForeColor = inputFg;

            lblTarget.Text = "포트 선택:"; lblTarget.Location = new System.Drawing.Point(115, 15); lblTarget.AutoSize = true; lblTarget.ForeColor = System.Drawing.Color.LightGray;
            cmbCom.SetBounds(115, 35, 120, 25); cmbCom.BackColor = inputBg; cmbCom.ForeColor = inputFg;
            cmbUsb.SetBounds(115, 35, 200, 25); cmbUsb.Visible = false; cmbUsb.BackColor = inputBg; cmbUsb.ForeColor = inputFg;
            txtIp.SetBounds(115, 35, 110, 25); txtIp.Visible = false; txtIp.BackColor = inputBg; txtIp.ForeColor = inputFg;

            lblTcpPort.Text = "포트:"; lblTcpPort.Location = new System.Drawing.Point(235, 15); lblTcpPort.AutoSize = true; lblTcpPort.ForeColor = System.Drawing.Color.LightGray; lblTcpPort.Visible = false;
            txtPort.SetBounds(235, 35, 50, 25); txtPort.Visible = false; txtPort.BackColor = inputBg; txtPort.ForeColor = inputFg;

            // COM 설정도 좌측으로 당김
            lblComSetup.Text = "COM 포트 설정:"; lblComSetup.Location = new System.Drawing.Point(15, 70); lblComSetup.AutoSize = true; lblComSetup.ForeColor = System.Drawing.Color.Orange;

            lblBaud.Text = "Baud:"; lblBaud.Location = new System.Drawing.Point(115, 70); lblBaud.AutoSize = true; lblBaud.ForeColor = System.Drawing.Color.LightGray;
            cmbBaudRate.Items.AddRange(new[] { "9600", "19200", "38400", "57600", "115200" }); cmbBaudRate.SelectedIndex = 0;
            cmbBaudRate.SetBounds(155, 68, 70, 25); cmbBaudRate.BackColor = inputBg; cmbBaudRate.ForeColor = inputFg;

            lblData.Text = "Data:"; lblData.Location = new System.Drawing.Point(235, 70); lblData.AutoSize = true; lblData.ForeColor = System.Drawing.Color.LightGray;
            cmbDataBits.Items.AddRange(new[] { "7", "8" }); cmbDataBits.SelectedIndex = 1;
            cmbDataBits.SetBounds(275, 68, 40, 25); cmbDataBits.BackColor = inputBg; cmbDataBits.ForeColor = inputFg;

            lblParity.Text = "Parity:"; lblParity.Location = new System.Drawing.Point(325, 70); lblParity.AutoSize = true; lblParity.ForeColor = System.Drawing.Color.LightGray;
            cmbParity.Items.AddRange(new[] { "None", "Odd", "Even", "Mark", "Space" }); cmbParity.SelectedIndex = 0;
            cmbParity.SetBounds(365, 68, 65, 25); cmbParity.BackColor = inputBg; cmbParity.ForeColor = inputFg;

            lblStop.Text = "Stop:"; lblStop.Location = new System.Drawing.Point(440, 70); lblStop.AutoSize = true; lblStop.ForeColor = System.Drawing.Color.LightGray;
            cmbStopBits.Items.AddRange(new[] { "None", "One", "Two", "OnePointFive" }); cmbStopBits.SelectedIndex = 1;
            cmbStopBits.SetBounds(480, 68, 80, 25); cmbStopBits.BackColor = inputBg; cmbStopBits.ForeColor = inputFg;

            cmbConnType.SelectedIndexChanged += (s, e) => {
                bool isCom = cmbConnType.SelectedIndex == 0;
                bool isUsb = cmbConnType.SelectedIndex == 1;
                bool isTcp = cmbConnType.SelectedIndex == 2;

                cmbCom.Visible = lblComSetup.Visible = cmbBaudRate.Visible = cmbDataBits.Visible = cmbParity.Visible = cmbStopBits.Visible = isCom;
                lblBaud.Visible = lblData.Visible = lblParity.Visible = lblStop.Visible = isCom;
                cmbUsb.Visible = isUsb;
                txtIp.Visible = txtPort.Visible = lblTcpPort.Visible = isTcp;

                lblTarget.Text = isCom ? "COM 포트:" : isUsb ? "USB 프린터:" : "IP 주소:";
            };

            pnlTop.Controls.Add(new System.Windows.Forms.Label { Text = "라벨 프로필 선택:", Location = new System.Drawing.Point(300, 15), AutoSize = true, ForeColor = System.Drawing.Color.LightGray });
            cmbProfile.SetBounds(300, 35, 190, 25);
            cmbProfile.BackColor = inputBg; cmbProfile.ForeColor = inputFg;
            cmbProfile.SelectedIndexChanged += new System.EventHandler(this.cmbProfile_SelectedIndexChanged);

            btnNewProf.Text = "+ 새 프로필";
            btnNewProf.SetBounds(505, 33, 85, 30);
            btnNewProf.BackColor = System.Drawing.Color.SeaGreen; btnNewProf.ForeColor = System.Drawing.Color.White;
            btnNewProf.Click += new System.EventHandler(this.btnNewProf_Click);

            btnEditProf.Text = "파라미터 편집";
            btnEditProf.SetBounds(600, 33, 100, 30);
            btnEditProf.BackColor = System.Drawing.Color.FromArgb(60, 70, 90);
            btnEditProf.Click += new System.EventHandler(this.btnEditProf_Click);

            btnDeleteProf.Text = "프로필 삭제";
            btnDeleteProf.SetBounds(710, 33, 90, 30);
            btnDeleteProf.BackColor = System.Drawing.Color.IndianRed; btnDeleteProf.ForeColor = System.Drawing.Color.White;
            btnDeleteProf.Click += new System.EventHandler(this.btnDeleteProf_Click);

            pnlTop.Controls.AddRange(new System.Windows.Forms.Control[] {
                cmbConnType, cmbCom, cmbUsb, txtIp, txtPort, lblTarget, lblTcpPort,
                lblComSetup, lblBaud, cmbBaudRate, lblData, cmbDataBits, lblParity, cmbParity, lblStop, cmbStopBits,
                cmbProfile, btnNewProf, btnEditProf, btnDeleteProf
            });

            // ── 옵션 영역 ──
            var pnlOptions = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Top, Height = 60, BackColor = System.Drawing.Color.FromArgb(25, 25, 35) };

            var lblScan = new System.Windows.Forms.Label { Text = "스캐너 입력:", Location = new System.Drawing.Point(15, 20), AutoSize = true };
            txtQuickScan.SetBounds(100, 18, 180, 25);
            txtQuickScan.BackColor = inputBg; txtQuickScan.ForeColor = inputFg;
            txtQuickScan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuickScan_KeyDown);

            chkShowBarcode.Text = "바코드 텍스트 출력"; chkShowBarcode.SetBounds(295, 20, 140, 25); chkShowBarcode.Checked = true;
            chkAutoPrint.Text = "스캔 즉시 자동발행"; chkAutoPrint.SetBounds(445, 20, 140, 25);
            var lblCopies = new System.Windows.Forms.Label { Text = "기본 매수:", Location = new System.Drawing.Point(595, 20), AutoSize = true };
            nudCopies.SetBounds(665, 18, 50, 25); nudCopies.Minimum = 1; nudCopies.Maximum = 99;
            nudCopies.BackColor = inputBg; nudCopies.ForeColor = inputFg;

            pnlOptions.Controls.AddRange(new System.Windows.Forms.Control[] { lblScan, txtQuickScan, chkShowBarcode, chkAutoPrint, lblCopies, nudCopies });

            // ── 그리드 ──
            dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvData.BackgroundColor = System.Drawing.Color.FromArgb(40, 40, 50);
            dgvData.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvData.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvData.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.DodgerBlue;
            dgvData.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellEndEdit);

            // ── 하단 영역 ──
            var pnlBot = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Bottom, Height = 60 };

            btnDeleteRow.Text = "선택 행 삭제"; btnDeleteRow.SetBounds(15, 10, 110, 40);
            btnDeleteRow.BackColor = System.Drawing.Color.IndianRed; btnDeleteRow.ForeColor = System.Drawing.Color.White;
            btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);

            btnPrintSelected.Text = "선택 인쇄"; btnPrintSelected.SetBounds(135, 10, 130, 40);
            btnPrintSelected.BackColor = System.Drawing.Color.DodgerBlue; btnPrintSelected.ForeColor = System.Drawing.Color.White;
            btnPrintSelected.Click += new System.EventHandler(this.btnPrintSelected_Click);

            btnPrintAll.Text = "전체 일괄 인쇄"; btnPrintAll.SetBounds(275, 10, 150, 40);
            btnPrintAll.BackColor = System.Drawing.Color.SeaGreen; btnPrintAll.ForeColor = System.Drawing.Color.White;
            btnPrintAll.Click += new System.EventHandler(this.btnPrintAll_Click);

            lblStatus.SetBounds(440, 20, 300, 25);
            lblStatus.ForeColor = System.Drawing.Color.LimeGreen;

            pnlBot.Controls.AddRange(new System.Windows.Forms.Control[] { btnDeleteRow, btnPrintSelected, btnPrintAll, lblStatus });

            this.Controls.Add(dgvData);
            this.Controls.Add(pnlOptions);
            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlBot);
            this.menuStripMain.SendToBack();
            this.ResumeLayout(false);
        }
    }
}