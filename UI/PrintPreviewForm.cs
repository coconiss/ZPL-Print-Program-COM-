using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZplPrinter.Core;

namespace ZplPrinter.UI
{
    public partial class PrintPreviewForm : Form
    {
        public PrintPreviewForm(string generatedZpl, Dictionary<string, string> values, LabelProfile profile)
        {
            InitializeComponent();
            foreach (var kvp in values) dgvInfo.Rows.Add(kvp.Key, kvp.Value);
            txtZpl.Text = generatedZpl;
        }

        private void btnOk_Click(object sender, EventArgs e) => DialogResult = DialogResult.OK;
        private void btnCancel_Click(object sender, EventArgs e) => DialogResult = DialogResult.Cancel;
    }
}