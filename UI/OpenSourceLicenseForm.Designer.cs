using System.Windows.Forms;

namespace ZplPrinter.UI
{
    partial class OpenSourceLicenseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbLicenseInfo = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            // 폼 기본 설정
            this.Text = "오픈소스 라이선스 및 외부 API 고지";
            this.Size = new System.Drawing.Size(650, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.FromArgb(20, 20, 30);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            // 라이선스 텍스트 박스 설정
            this.rtbLicenseInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLicenseInfo.BackColor = System.Drawing.Color.FromArgb(40, 40, 50);
            this.rtbLicenseInfo.ForeColor = System.Drawing.Color.LightGray;
            this.rtbLicenseInfo.ReadOnly = true;
            this.rtbLicenseInfo.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular);
            this.rtbLicenseInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;

            // 여백 처리를 위한 패널 컨테이너
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            panel.Controls.Add(this.rtbLicenseInfo);

            this.Controls.Add(panel);
            this.ResumeLayout(false);
        }

        #endregion
    }
}