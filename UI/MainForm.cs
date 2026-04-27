using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Windows.Forms;
using ZplPrinter.Core;

namespace ZplPrinter.UI
{
    public partial class MainForm : Form
    {
        private List<LabelProfile> _profiles = new();
        private LabelProfile _activeProfile = null!;

        public MainForm()
        {
            InitializeComponent();
            LoadConnections();
            RefreshProfiles();

            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            var s = SettingsManager.Load();

            if (s.LastConnType >= 0 && s.LastConnType < cmbConnType.Items.Count)
                cmbConnType.SelectedIndex = s.LastConnType;

            SetComboIfContains(cmbCom, s.LastComPort);
            SetComboIfContains(cmbBaudRate, s.LastBaudRate);
            SetComboIfContains(cmbDataBits, s.LastDataBits);
            SetComboIfContains(cmbParity, s.LastParity);
            SetComboIfContains(cmbStopBits, s.LastStopBits);
            SetComboIfContains(cmbUsb, s.LastUsbPrinter);

            txtIp.Text = s.LastIp;
            txtPort.Text = s.LastTcpPort;

            SetComboIfContains(cmbProfile, s.LastProfileName);

            chkShowBarcode.Checked = s.ShowBarcode;
            chkAutoPrint.Checked = s.AutoPrint;

            if (s.DefaultCopies > 0 && s.DefaultCopies <= nudCopies.Maximum)
                nudCopies.Value = s.DefaultCopies;
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            var s = new AppSettings
            {
                LastConnType = cmbConnType.SelectedIndex,
                LastComPort = cmbCom.Text,
                LastBaudRate = cmbBaudRate.Text,
                LastDataBits = cmbDataBits.Text,
                LastParity = cmbParity.Text,
                LastStopBits = cmbStopBits.Text,
                LastUsbPrinter = cmbUsb.Text,
                LastIp = txtIp.Text,
                LastTcpPort = txtPort.Text,
                LastProfileName = cmbProfile.Text,
                ShowBarcode = chkShowBarcode.Checked,
                AutoPrint = chkAutoPrint.Checked,
                DefaultCopies = (int)nudCopies.Value
            };
            SettingsManager.Save(s);
        }

        private void SetComboIfContains(ComboBox cmb, string val)
        {
            if (!string.IsNullOrEmpty(val) && cmb.Items.Contains(val))
                cmb.SelectedItem = val;
            else if (cmb.Items.Count > 0 && cmb.SelectedIndex == -1)
                cmb.SelectedIndex = 0;
        }

        private void LoadConnections()
        {
            cmbCom.Items.AddRange(SerialPort.GetPortNames());
            if (cmbCom.Items.Count > 0) cmbCom.SelectedIndex = 0;

            foreach (string p in PrinterSettings.InstalledPrinters) cmbUsb.Items.Add(p);
            if (cmbUsb.Items.Count > 0) cmbUsb.SelectedIndex = 0;
        }

        private void RefreshProfiles()
        {
            _profiles = ProfileManager.LoadAll();
            cmbProfile.Items.Clear();
            foreach (var p in _profiles) cmbProfile.Items.Add(p.Name);
            if (_profiles.Count > 0) cmbProfile.SelectedIndex = 0;
        }

        private void cmbProfile_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbProfile.SelectedIndex < 0) return;
            _activeProfile = _profiles[cmbProfile.SelectedIndex];
            BuildGridColumns();
        }

        private void BuildGridColumns()
        {
            dgvData.Columns.Clear();
            foreach (var f in _activeProfile.Fields)
                dgvData.Columns.Add(f.Key, f.DisplayName);

            dgvData.Columns.Add("COPIES", "발행 매수");
        }

        private void btnNewProf_Click(object sender, EventArgs e)
        {
            var newProfile = new LabelProfile { Name = "새 라벨 프로필" };
            using var frm = new ProfileEditorForm(newProfile);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ProfileManager.Save(newProfile);
                RefreshProfiles();
                cmbProfile.SelectedItem = newProfile.Name;
            }
        }

        private void btnEditProf_Click(object? sender, EventArgs e)
        {
            if (_activeProfile == null) return;
            using var frm = new ProfileEditorForm(_activeProfile);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ProfileManager.Save(_activeProfile);
                RefreshProfiles();
                cmbProfile.SelectedItem = _activeProfile.Name;
            }
        }

        // [신규] 프로필 삭제 로직
        private void btnDeleteProf_Click(object? sender, EventArgs e)
        {
            if (_activeProfile == null) return;

            if (_profiles.Count <= 1)
            {
                MessageBox.Show("최소 1개의 라벨 프로필은 유지해야 합니다.", "삭제 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"정말 '{_activeProfile.Name}' 프로필을 삭제하시겠습니까?", "프로필 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ProfileManager.Delete(_activeProfile);
                RefreshProfiles(); // 삭제 후 목록 갱신 (자동으로 첫 번째 프로필 선택됨)
            }
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvData.SelectedRows)
            {
                if (!row.IsNewRow) dgvData.Rows.Remove(row);
            }
        }

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || _activeProfile == null) return;

            string colKey = dgvData.Columns[e.ColumnIndex].Name;
            var field = _activeProfile.Fields.Find(f => f.Key == colKey);
            var cell = dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (field != null && cell.Value != null)
            {
                string cellStr = cell.Value.ToString() ?? "";

                var targetField = _activeProfile.Fields.Find(f => f.Key == cellStr);
                if (targetField != null && dgvData.Columns.Contains(targetField.Key))
                {
                    var targetCellVal = dgvData.Rows[e.RowIndex].Cells[targetField.Key].Value;
                    if (targetCellVal != null)
                    {
                        cellStr = targetCellVal.ToString()!.Replace("-", "");
                    }
                }

                if (field.UseSegment && !string.IsNullOrEmpty(field.SegmentFormat))
                {
                    cell.Value = LabelProfile.ApplySegment(cellStr, field.SegmentFormat);
                }
                else
                {
                    cell.Value = cellStr;
                }
            }
        }

        private void txtQuickScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _activeProfile != null)
            {
                e.Handled = true; e.SuppressKeyPress = true;
                string scannedValue = txtQuickScan.Text.Trim();
                if (string.IsNullOrEmpty(scannedValue)) return;

                int rIdx = dgvData.Rows.Add();

                var rawValues = new Dictionary<string, string>();
                for (int i = 0; i < _activeProfile.Fields.Count; i++)
                {
                    string key = _activeProfile.Fields[i].Key;
                    rawValues[key] = (i == 0) ? scannedValue : _activeProfile.Fields[i].DefaultValue;
                }

                for (int i = 0; i < _activeProfile.Fields.Count; i++)
                {
                    var field = _activeProfile.Fields[i];
                    string finalVal = rawValues[field.Key];

                    if (rawValues.ContainsKey(finalVal))
                    {
                        finalVal = rawValues[finalVal];
                    }

                    if (field.UseSegment && !string.IsNullOrEmpty(field.SegmentFormat))
                    {
                        finalVal = LabelProfile.ApplySegment(finalVal, field.SegmentFormat);
                    }

                    dgvData.Rows[rIdx].Cells[i].Value = finalVal;
                }

                dgvData.Rows[rIdx].Cells["COPIES"].Value = nudCopies.Value.ToString();
                txtQuickScan.Clear();

                if (chkAutoPrint.Checked)
                {
                    dgvData.ClearSelection();
                    dgvData.Rows[rIdx].Selected = true;
                    if (PrintRows(new[] { dgvData.Rows[rIdx] }))
                    {
                        dgvData.Rows.Clear();
                    }
                }
            }
        }

        private void btnPrintSelected_Click(object sender, EventArgs e)
        {
            if (PrintRows(dgvData.SelectedRows))
            {
                foreach (DataGridViewRow r in dgvData.SelectedRows)
                    if (!r.IsNewRow) dgvData.Rows.Remove(r);
            }
        }

        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            var rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in dgvData.Rows) if (!r.IsNewRow) rows.Add(r);

            if (PrintRows(rows)) dgvData.Rows.Clear();
        }

        private bool PrintRows(System.Collections.IEnumerable rows)
        {
            int successCnt = 0;
            bool showBarcode = chkShowBarcode.Checked;

            foreach (DataGridViewRow row in rows)
            {
                if (row.IsNewRow) continue;
                var values = new Dictionary<string, string>();
                foreach (var f in _activeProfile.Fields)
                    values[f.Key] = row.Cells[f.Key].Value?.ToString() ?? "";

                int.TryParse(row.Cells["COPIES"].Value?.ToString(), out int copies);
                if (copies <= 0) copies = 1;

                string zpl = _activeProfile.BuildZpl(values, copies, showBarcode);

                if (!chkAutoPrint.Checked)
                {
                    using var preview = new PrintPreviewForm(zpl, values, _activeProfile);
                    if (preview.ShowDialog() != DialogResult.OK) return false;
                }

                bool ok = false; string err = "";
                if (cmbConnType.SelectedIndex == 0)
                {
                    int baudRate = int.Parse(cmbBaudRate.Text);
                    int dataBits = int.Parse(cmbDataBits.Text);
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);
                    StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.Text);
                    ok = PrinterService.PrintViaCom(cmbCom.Text, baudRate, dataBits, parity, stopBits, zpl, out err);
                }
                else if (cmbConnType.SelectedIndex == 1) ok = PrinterService.PrintViaUsb(cmbUsb.Text, zpl, out err);
                else if (cmbConnType.SelectedIndex == 2) ok = PrinterService.PrintViaEthernet(txtIp.Text, int.Parse(txtPort.Text), zpl, out err);

                if (ok) { successCnt++; System.Threading.Thread.Sleep(_activeProfile.PrintDelayMs); }
                else { MessageBox.Show($"오류: {err}", "인쇄 실패", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
            }
            lblStatus.Text = $"{successCnt}건 발행 완료";
            return successCnt > 0;
        }

        private void menuOpenSource_Click(object sender, EventArgs e)
        {
            using var licenseForm = new OpenSourceLicenseForm();
            licenseForm.ShowDialog(this); // 현재 메인 폼을 부모로 하여 모달(Modal) 창으로 띄움
        }
    }
}