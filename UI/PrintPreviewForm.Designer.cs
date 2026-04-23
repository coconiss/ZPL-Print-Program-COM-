namespace ZplPrinter.UI
{
    partial class PrintPreviewForm
    {
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.TextBox txtZpl;
        private System.Windows.Forms.Button btnOk, btnCancel;

        private void InitializeComponent()
        {
            this.dgvInfo = new System.Windows.Forms.DataGridView();
            this.txtZpl = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.SuspendLayout();
            this.Size = new System.Drawing.Size(600, 500);
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 28);
            this.ForeColor = System.Drawing.Color.White;

            dgvInfo.SetBounds(20, 20, 540, 150);
            dgvInfo.BackgroundColor = System.Drawing.Color.FromArgb(40, 40, 50);
            dgvInfo.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvInfo.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvInfo.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            dgvInfo.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvInfo.EnableHeadersVisualStyles = false;

            dgvInfo.Columns.Add("K", "파라미터");
            dgvInfo.Columns.Add("V", "값");

            txtZpl.Multiline = true;
            txtZpl.ReadOnly = true;
            txtZpl.SetBounds(20, 180, 540, 220);
            txtZpl.BackColor = System.Drawing.Color.Black;
            txtZpl.ForeColor = System.Drawing.Color.Lime;

            btnOk.Text = "전송"; btnOk.SetBounds(460, 410, 100, 35);
            btnOk.BackColor = System.Drawing.Color.DodgerBlue; btnOk.ForeColor = System.Drawing.Color.White;
            btnOk.Click += new System.EventHandler(this.btnOk_Click);

            btnCancel.Text = "취소"; btnCancel.SetBounds(350, 410, 100, 35);
            btnCancel.BackColor = System.Drawing.Color.IndianRed; btnCancel.ForeColor = System.Drawing.Color.White;
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] { dgvInfo, txtZpl, btnOk, btnCancel });
            this.ResumeLayout(false);
        }
    }
}