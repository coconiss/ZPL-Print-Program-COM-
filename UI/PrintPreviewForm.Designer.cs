namespace ZplPrinter.UI
{
    partial class PrintPreviewForm
    {
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Button btnOk, btnCancel;
        private System.Windows.Forms.Label lblLoading;

        private void InitializeComponent()
        {
            this.dgvInfo = new System.Windows.Forms.DataGridView();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblLoading = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();

            this.Text = "실제 라벨 인쇄 미리보기 (Labelary API)";
            this.Size = new System.Drawing.Size(900, 550); // 이미지 렌더링을 위해 가로폭 확장
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 28);
            this.ForeColor = System.Drawing.Color.White;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            // ── 좌측: 매핑된 파라미터 값 정보 (폭 고정) ──
            dgvInfo.SetBounds(15, 15, 250, 420);
            dgvInfo.BackgroundColor = System.Drawing.Color.FromArgb(40, 40, 50);
            dgvInfo.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvInfo.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvInfo.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            dgvInfo.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvInfo.EnableHeadersVisualStyles = false;
            dgvInfo.RowHeadersVisible = false;
            dgvInfo.AllowUserToAddRows = false;
            dgvInfo.ReadOnly = true;
            dgvInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            dgvInfo.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "K", HeaderText = "파라미터", Width = 90 });
            dgvInfo.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "V", HeaderText = "데이터", AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill });

            // ── 우측: 라벨 이미지 미리보기 창 ──
            picPreview.SetBounds(280, 15, 585, 420);
            picPreview.BackColor = System.Drawing.Color.White;
            picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom; // 비율 유지하며 꽉 차게 렌더링
            picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // 로딩 상태 라벨 (이미지 로드 전 표시)
            lblLoading.Text = "라벨 디자인 렌더링 중...\n(인터넷 연결 필요)";
            lblLoading.AutoSize = false;
            lblLoading.SetBounds(280, 15, 585, 420);
            lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblLoading.BackColor = System.Drawing.Color.FromArgb(25, 25, 35);
            lblLoading.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular);
            lblLoading.ForeColor = System.Drawing.Color.LightGray;

            // ── 하단 버튼 영역 ──
            btnOk.Text = "인쇄 승인";
            btnOk.SetBounds(765, 455, 100, 40);
            btnOk.BackColor = System.Drawing.Color.DodgerBlue;
            btnOk.ForeColor = System.Drawing.Color.White;
            btnOk.Click += new System.EventHandler(this.btnOk_Click);

            btnCancel.Text = "취소";
            btnCancel.SetBounds(650, 455, 100, 40);
            btnCancel.BackColor = System.Drawing.Color.IndianRed;
            btnCancel.ForeColor = System.Drawing.Color.White;
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                dgvInfo, lblLoading, picPreview, btnOk, btnCancel
            });

            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);
        }
    }
}