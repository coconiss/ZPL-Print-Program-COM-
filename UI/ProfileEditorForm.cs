using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZplPrinter.Core;

namespace ZplPrinter.UI
{
    public partial class ProfileEditorForm : Form
    {
        private LabelProfile _profile;

        public ProfileEditorForm(LabelProfile profile)
        {
            _profile = profile;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            txtName.Text = _profile.Name;
            txtW.Text = _profile.LabelWidthMm.ToString();
            txtH.Text = _profile.LabelHeightMm.ToString();
            txtDpi.Text = _profile.Dpi.ToString();
            txtZpl.Text = _profile.ZplTemplate;
            txtZplOnly.Text = _profile.ZplTemplateEncodingOnly;

            foreach (var f in _profile.Fields)
                dgvFields.Rows.Add(f.Key, f.DisplayName, f.DefaultValue, f.UseSegment, f.SegmentFormat);
        }

        // [신규] 선택 행 위로 이동
        private void btnRowUp_Click(object sender, EventArgs e)
        {
            if (dgvFields.SelectedRows.Count == 0) return;
            int rowIndex = dgvFields.SelectedRows[0].Index;

            // 첫 번째 행이거나 새 행(입력 대기열)이면 이동 불가
            if (rowIndex <= 0 || dgvFields.Rows[rowIndex].IsNewRow) return;

            var row = dgvFields.Rows[rowIndex];
            dgvFields.Rows.Remove(row);
            dgvFields.Rows.Insert(rowIndex - 1, row);
            dgvFields.ClearSelection();
            dgvFields.Rows[rowIndex - 1].Selected = true;
        }

        // [신규] 선택 행 아래로 이동
        private void btnRowDown_Click(object sender, EventArgs e)
        {
            if (dgvFields.SelectedRows.Count == 0) return;
            int rowIndex = dgvFields.SelectedRows[0].Index;

            // 마지막 행이거나 새 행이면 이동 불가
            if (rowIndex >= dgvFields.Rows.Count - 1 || dgvFields.Rows[rowIndex].IsNewRow) return;
            // 바로 다음 행이 새 행(입력 대기열)이어도 이동 불가
            if (dgvFields.Rows[rowIndex + 1].IsNewRow) return;

            var row = dgvFields.Rows[rowIndex];
            dgvFields.Rows.Remove(row);
            dgvFields.Rows.Insert(rowIndex + 1, row);
            dgvFields.ClearSelection();
            dgvFields.Rows[rowIndex + 1].Selected = true;
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            var matches = Regex.Matches(txtZpl.Text, @"\{([a-zA-Z0-9_]+)\}");
            var existingKeys = new HashSet<string>();

            foreach (DataGridViewRow r in dgvFields.Rows)
            {
                if (!r.IsNewRow && r.Cells[0].Value != null)
                    existingKeys.Add(r.Cells[0].Value.ToString()!);
            }

            int addedCount = 0;
            foreach (Match m in matches)
            {
                string key = m.Groups[1].Value;
                if (key == "COPIES" || key == "WIDTH_DOTS" || key == "HEIGHT_DOTS") continue;

                if (!existingKeys.Contains(key))
                {
                    dgvFields.Rows.Add(key, key, "", false, "");
                    existingKeys.Add(key);
                    addedCount++;
                }
            }
            MessageBox.Show($"{addedCount}개의 파라미터가 추가되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _profile.Name = txtName.Text;
            if (double.TryParse(txtW.Text, out double w)) _profile.LabelWidthMm = w;
            if (double.TryParse(txtH.Text, out double h)) _profile.LabelHeightMm = h;
            if (int.TryParse(txtDpi.Text, out int dpi)) _profile.Dpi = dpi;

            _profile.ZplTemplate = txtZpl.Text;
            _profile.ZplTemplateEncodingOnly = txtZplOnly.Text;

            _profile.Fields.Clear();
            foreach (DataGridViewRow row in dgvFields.Rows)
            {
                if (row.IsNewRow) continue;
                string key = row.Cells[0].Value?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(key)) continue;

                _profile.Fields.Add(new ProfileField
                {
                    Key = key,
                    DisplayName = row.Cells[1].Value?.ToString() ?? key,
                    DefaultValue = row.Cells[2].Value?.ToString() ?? "",
                    UseSegment = Convert.ToBoolean(row.Cells[3].Value),
                    SegmentFormat = row.Cells[4].Value?.ToString() ?? ""
                });
            }
            DialogResult = DialogResult.OK;
        }
    }
}