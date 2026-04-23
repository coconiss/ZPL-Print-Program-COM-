namespace ZplPrinter.UI
{
    partial class ProfileEditorForm
    {
        private System.Windows.Forms.TextBox txtName, txtW, txtH, txtDpi, txtZpl, txtZplOnly;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.Button btnSave, btnExtract;

        // 추가: 행 이동 버튼
        private System.Windows.Forms.Button btnRowUp, btnRowDown;

        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtW = new System.Windows.Forms.TextBox();
            this.txtH = new System.Windows.Forms.TextBox();
            this.txtDpi = new System.Windows.Forms.TextBox();
            this.txtZpl = new System.Windows.Forms.TextBox();
            this.txtZplOnly = new System.Windows.Forms.TextBox();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();

            this.btnRowUp = new System.Windows.Forms.Button();
            this.btnRowDown = new System.Windows.Forms.Button();

            this.SuspendLayout();

            this.Text = "라벨 파라미터 설계 및 편집";
            this.Size = new System.Drawing.Size(1150, 770);
            this.BackColor = System.Drawing.Color.FromArgb(20, 20, 30);
            this.ForeColor = System.Drawing.Color.White;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var inputBg = System.Drawing.Color.FromArgb(50, 50, 60);
            var inputFg = System.Drawing.Color.White;

            // ── 상단 프로필 기본 정보 ──
            this.Controls.Add(new System.Windows.Forms.Label { Text = "프로필 제목:", Location = new System.Drawing.Point(20, 23), AutoSize = true, ForeColor = System.Drawing.Color.LightGray });
            txtName.SetBounds(105, 20, 180, 25); txtName.BackColor = inputBg; txtName.ForeColor = inputFg;

            this.Controls.Add(new System.Windows.Forms.Label { Text = "가로(mm):", Location = new System.Drawing.Point(20, 58), AutoSize = true, ForeColor = System.Drawing.Color.LightGray });
            txtW.SetBounds(105, 55, 60, 25); txtW.BackColor = inputBg; txtW.ForeColor = inputFg;

            this.Controls.Add(new System.Windows.Forms.Label { Text = "세로(mm):", Location = new System.Drawing.Point(175, 58), AutoSize = true, ForeColor = System.Drawing.Color.LightGray });
            txtH.SetBounds(250, 55, 60, 25); txtH.BackColor = inputBg; txtH.ForeColor = inputFg;

            this.Controls.Add(new System.Windows.Forms.Label { Text = "해상도(DPI):", Location = new System.Drawing.Point(20, 93), AutoSize = true, ForeColor = System.Drawing.Color.LightGray });
            txtDpi.SetBounds(105, 90, 60, 25); txtDpi.BackColor = inputBg; txtDpi.ForeColor = inputFg;

            // ── 동적 파라미터 그리드 ──
            this.Controls.Add(new System.Windows.Forms.Label { Text = "동적 파라미터 설계 (다른 변수명 입력시 값 참조)", Location = new System.Drawing.Point(320, 20), AutoSize = true, ForeColor = System.Drawing.Color.Orange });

            dgvFields.SetBounds(320, 45, 780, 170);
            dgvFields.BackgroundColor = System.Drawing.Color.FromArgb(40, 40, 50);
            dgvFields.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvFields.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;

            // 순서 이동을 위해 선택 모드를 '전체 행 선택'으로 변경
            dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvFields.MultiSelect = false;

            dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvFields.ColumnHeadersHeight = 40;
            dgvFields.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            dgvFields.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvFields.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            dgvFields.EnableHeadersVisualStyles = false;

            dgvFields.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "K", HeaderText = "ZPL 내부 변수명", Width = 150 });
            dgvFields.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "D", HeaderText = "UI 표기명", Width = 150 });
            dgvFields.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "V", HeaderText = "기본값(또는 참조 변수명)", Width = 210 });

            var chkCol = new System.Windows.Forms.DataGridViewCheckBoxColumn { Name = "UseSeg", HeaderText = "세그먼트 사용", Width = 110 };
            dgvFields.Columns.Add(chkCol);
            dgvFields.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "SegFmt", HeaderText = "분할(예: 3,7,2)", Width = 130 });

            // 추출 버튼
            btnExtract.Text = "ZPL에서 변수명 자동 추출 ↓";
            btnExtract.SetBounds(320, 225, 200, 30);
            btnExtract.BackColor = System.Drawing.Color.SeaGreen; btnExtract.ForeColor = System.Drawing.Color.White;
            btnExtract.Click += new System.EventHandler(this.btnExtract_Click);

            // ── 순서 이동 버튼 (위/아래) 추가 ──
            btnRowUp.Text = "▲ 위로";
            btnRowUp.SetBounds(935, 225, 80, 30);
            btnRowUp.BackColor = System.Drawing.Color.FromArgb(80, 90, 110);
            btnRowUp.ForeColor = System.Drawing.Color.White;
            btnRowUp.Click += new System.EventHandler(this.btnRowUp_Click);

            btnRowDown.Text = "▼ 아래로";
            btnRowDown.SetBounds(1020, 225, 80, 30);
            btnRowDown.BackColor = System.Drawing.Color.FromArgb(80, 90, 110);
            btnRowDown.ForeColor = System.Drawing.Color.White;
            btnRowDown.Click += new System.EventHandler(this.btnRowDown_Click);

            // ── ZPL 템플릿 입력창 ──
            var lblZpl = new System.Windows.Forms.Label { Text = "ZPL 템플릿 (바코드 텍스트 포함):", Location = new System.Drawing.Point(20, 260), AutoSize = true, ForeColor = System.Drawing.Color.LightGray };
            txtZpl.Multiline = true;
            txtZpl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtZpl.SetBounds(20, 280, 1080, 180);
            txtZpl.BackColor = System.Drawing.Color.Black; txtZpl.ForeColor = System.Drawing.Color.Lime;

            var lblZplOnly = new System.Windows.Forms.Label { Text = "ZPL 템플릿 (인코딩 전용 / 화면 출력 없음):", Location = new System.Drawing.Point(20, 475), AutoSize = true, ForeColor = System.Drawing.Color.LightGray };
            txtZplOnly.Multiline = true;
            txtZplOnly.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtZplOnly.SetBounds(20, 495, 1080, 180);
            txtZplOnly.BackColor = System.Drawing.Color.Black; txtZplOnly.ForeColor = System.Drawing.Color.Orange;

            // ── 저장 버튼 ──
            btnSave.Text = "저장";
            btnSave.SetBounds(1000, 690, 100, 40);
            btnSave.BackColor = System.Drawing.Color.DodgerBlue;
            btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                txtName, txtW, txtH, txtDpi, dgvFields, btnExtract,
                btnRowUp, btnRowDown, // 추가된 버튼
                lblZpl, txtZpl, lblZplOnly, txtZplOnly, btnSave
            });
            this.ResumeLayout(false);
        }
    }
}