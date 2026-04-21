using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace ZplPrinter
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;
        private const string SettingsFileName = "user_settings.json";

        // 프로필 상태
        private List<LabelProfile> _profiles = new();
        private LabelProfile _activeProfile = ProfileManager.CreateDefaultProfile();

        // 스캔 목록
        private readonly List<string> _scannedBarcodes = new();

        // ══════════════════════════════════════════════════════════════════
        // USB RAW 인쇄용 Win32 P/Invoke
        // ══════════════════════════════════════════════════════════════════
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DOCINFO
        {
            [MarshalAs(UnmanagedType.LPTStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPTStr)] public string? pOutputFile;
            [MarshalAs(UnmanagedType.LPTStr)] public string pDataType;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr hPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO di);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        // ── 초기화 ────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
            LoadInstalledPrinters();
            LoadProfiles();       // 프로필 먼저 로드
            LoadUserSettings();   // 저장된 설정(프로필 선택 포함) 복원

            this.Shown += Form1_Shown;
            txtBarcode.KeyDown += txtBarcode_KeyDown;

            // 설정 변경 시 자동 저장
            cmbPort.SelectedIndexChanged += (s, e) => SaveUserSettings();
            cmbPrinter.SelectedIndexChanged += (s, e) => SaveUserSettings();
            txtIpAddress.TextChanged += (s, e) => SaveUserSettings();
            txtEthPort.TextChanged += (s, e) => SaveUserSettings();
            chkShowBarcode.CheckedChanged += (s, e) => SaveUserSettings();
            chkAutoPrint.CheckedChanged += (s, e) => SaveUserSettings();
            nudCopies.ValueChanged += (s, e) => SaveUserSettings();
            rdoCom.CheckedChanged += (s, e) => SaveUserSettings();
            rdoUsb.CheckedChanged += (s, e) => SaveUserSettings();
            rdoEthernet.CheckedChanged += (s, e) => SaveUserSettings();
        }

        private void Form1_Shown(object sender, EventArgs e) => txtBarcode.Focus();

        // ── 연결 방식 전환 ─────────────────────────────────────────────────
        private void rdoConnect_CheckedChanged(object sender, EventArgs e)
        {
            pnlCom.Visible = rdoCom.Checked;
            pnlUsb.Visible = rdoUsb.Checked;
            pnlEthernet.Visible = rdoEthernet.Checked;

            string mode = rdoCom.Checked ? "COM 시리얼"
                        : rdoUsb.Checked ? "USB (RAW)"
                                         : "Ethernet (TCP/IP)";
            SetStatus($"연결 방식: {mode}", success: true);
            SaveUserSettings();
        }

        // ══════════════════════════════════════════════════════════════════
        // 프로필 관리
        // ══════════════════════════════════════════════════════════════════

        private void LoadProfiles()
        {
            _profiles = ProfileManager.LoadAll();

            cmbProfile.BeginUpdate();
            cmbProfile.Items.Clear();
            foreach (var p in _profiles)
                cmbProfile.Items.Add(p.Name);
            cmbProfile.EndUpdate();

            if (_profiles.Count > 0)
                cmbProfile.SelectedIndex = 0;
            // SelectedIndexChanged → _activeProfile 설정 + ApplyProfileToUI
        }

        private void cmbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cmbProfile.SelectedIndex;
            if (idx < 0 || idx >= _profiles.Count) return;
            _activeProfile = _profiles[idx];
            ApplyProfileToUI();
            SaveUserSettings();
        }

        /// <summary>
        /// 선택된 프로필의 설정을 UI에 반영한다.
        /// 인쇄 옵션(ShowBarcode, Copies, AutoPrint)은 Form1 자체 설정이므로 건드리지 않는다.
        /// </summary>
        private void ApplyProfileToUI()
        {
            // 라벨 크기 표시 갱신
            lblLabelSizeValue.Text =
                $"{_activeProfile.LabelWidthMm:F1} × {_activeProfile.LabelHeightMm:F1} mm  " +
                $"|  {_activeProfile.Dpi} DPI  " +
                $"|  딜레이 {_activeProfile.PrintDelayMs} ms";

            // 바코드 그룹 타이틀 및 힌트 갱신
            grpBarcode.Text = $"바코드 입력 ({_activeProfile.BarcodeInputLength}자리 숫자)";
            string example = new string('0', _activeProfile.BarcodeInputLength);
            lblBarcodeHint.Text =
                $"입력: {_activeProfile.BarcodeInputLength}자리  →  표시: {_activeProfile.FormatBarcode(example)}";

            // 바코드 텍스트박스 최대 길이 갱신
            txtBarcode.MaxLength = _activeProfile.BarcodeInputLength;
            if (txtBarcode.Text.Length > _activeProfile.BarcodeInputLength)
                txtBarcode.Clear();

            SetStatus($"프로필: {_activeProfile.Name}", success: true);
        }

        private void btnNewProfile_Click(object sender, EventArgs e)
        {
            var newProfile = new LabelProfile();
            using var dlg = new ProfileEditorForm(newProfile, isNew: true);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            ProfileManager.Save(newProfile);
            _profiles.Add(newProfile);
            RefreshProfileCombo(newProfile.Id);
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            int idx = cmbProfile.SelectedIndex;
            if (idx < 0 || idx >= _profiles.Count) return;

            // 딥 카피로 편집 → 취소 시 원본 유지
            var copy = ProfileManager.DeepCopy(_profiles[idx]);
            using var dlg = new ProfileEditorForm(copy, isNew: false);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            copy.Id = _profiles[idx].Id;   // ID 보존
            _profiles[idx] = copy;
            ProfileManager.Save(copy);
            _activeProfile = copy;
            RefreshProfileCombo(copy.Id);
            ApplyProfileToUI();
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (_profiles.Count <= 1)
            {
                MessageBox.Show("마지막 프로필은 삭제할 수 없습니다.",
                    "삭제 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = cmbProfile.SelectedIndex;
            if (idx < 0) return;
            var profile = _profiles[idx];

            if (MessageBox.Show($"'{profile.Name}' 프로필을 삭제하시겠습니까?",
                    "프로필 삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                != DialogResult.OK) return;

            ProfileManager.Delete(profile);
            _profiles.RemoveAt(idx);
            cmbProfile.Items.RemoveAt(idx);
            cmbProfile.SelectedIndex = Math.Min(idx, _profiles.Count - 1);
        }

        /// <summary>
        /// 프로필 목록을 이름순 정렬 후 ComboBox를 재구성한다.
        /// selectId 에 해당하는 항목을 선택한다.
        /// </summary>
        private void RefreshProfileCombo(string selectId)
        {
            _profiles.Sort((a, b) =>
                string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            cmbProfile.BeginUpdate();
            cmbProfile.Items.Clear();
            int selIdx = 0;
            for (int i = 0; i < _profiles.Count; i++)
            {
                cmbProfile.Items.Add(_profiles[i].Name);
                if (_profiles[i].Id == selectId) selIdx = i;
            }
            cmbProfile.EndUpdate();
            cmbProfile.SelectedIndex = selIdx;
        }

        // ══════════════════════════════════════════════════════════════════
        // 설정 저장 / 불러오기
        // ══════════════════════════════════════════════════════════════════
        private sealed class UserSettings
        {
            public string? ConnectMode { get; set; }
            public string? PortName { get; set; }
            public string? PrinterName { get; set; }
            public string? IpAddress { get; set; }
            public string? EthPort { get; set; }
            public bool ShowBarcode { get; set; }
            public bool AutoPrint { get; set; }
            public int Copies { get; set; } = 1;
            public string? SelectedProfileId { get; set; }
        }

        private string GetSettingsPath()
            => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);

        private void SaveUserSettings()
        {
            try
            {
                var s = new UserSettings
                {
                    ConnectMode = rdoCom.Checked ? "COM" : rdoUsb.Checked ? "USB" : "ETH",
                    PortName = cmbPort.SelectedItem?.ToString(),
                    PrinterName = cmbPrinter.SelectedItem?.ToString(),
                    IpAddress = txtIpAddress.Text,
                    EthPort = txtEthPort.Text,
                    ShowBarcode = chkShowBarcode.Checked,
                    AutoPrint = chkAutoPrint.Checked,
                    Copies = (int)nudCopies.Value,
                    SelectedProfileId = _activeProfile.Id
                };

                string dir = Path.GetDirectoryName(GetSettingsPath())!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                File.WriteAllText(GetSettingsPath(),
                    JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true }),
                    Encoding.UTF8);
            }
            catch { /* 실패해도 앱 동작에 영향 없음 */ }
        }

        private void LoadUserSettings()
        {
            try
            {
                string path = GetSettingsPath();
                if (!File.Exists(path)) return;

                var s = JsonSerializer.Deserialize<UserSettings>(
                    File.ReadAllText(path, Encoding.UTF8));
                if (s == null) return;

                if (s.ConnectMode == "COM") rdoCom.Checked = true;
                else if (s.ConnectMode == "USB") rdoUsb.Checked = true;
                else if (s.ConnectMode == "ETH") rdoEthernet.Checked = true;

                SelectComboItem(cmbPort, s.PortName);
                SelectComboItem(cmbPrinter, s.PrinterName);

                if (!string.IsNullOrWhiteSpace(s.IpAddress)) txtIpAddress.Text = s.IpAddress;
                if (!string.IsNullOrWhiteSpace(s.EthPort)) txtEthPort.Text = s.EthPort;

                chkShowBarcode.Checked = s.ShowBarcode;
                chkAutoPrint.Checked = s.AutoPrint;
                nudCopies.Value = Math.Max(1, Math.Min(99, s.Copies));

                // 저장된 프로필 복원
                if (!string.IsNullOrEmpty(s.SelectedProfileId))
                {
                    int idx = _profiles.FindIndex(p => p.Id == s.SelectedProfileId);
                    if (idx >= 0 && idx != cmbProfile.SelectedIndex)
                        cmbProfile.SelectedIndex = idx;   // → cmbProfile_SelectedIndexChanged 발생
                }
            }
            catch { /* ignore */ }
        }

        private static void SelectComboItem(ComboBox cmb, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            for (int i = 0; i < cmb.Items.Count; i++)
                if (cmb.Items[i]?.ToString() == value) { cmb.SelectedIndex = i; return; }
        }

        // ══════════════════════════════════════════════════════════════════
        // COM 시리얼
        // ══════════════════════════════════════════════════════════════════
        private void LoadAvailablePorts()
        {
            cmbPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0) { cmbPort.Items.AddRange(ports); cmbPort.SelectedIndex = 0; }
            else { cmbPort.Items.Add("사용 가능한 포트 없음"); cmbPort.SelectedIndex = 0; }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAvailablePorts();
            SetStatus("COM 포트 목록을 새로고침했습니다.", success: true);
        }

        private bool PrintViaCom(string zpl, string barcodeDisplay, bool showMessageBox = true)
        {
            if (cmbPort.SelectedItem?.ToString() == "사용 가능한 포트 없음"
                || cmbPort.SelectedItem == null)
            {
                ShowError("유효한 COM 포트를 선택해주세요."); return false;
            }

            string portName = cmbPort.SelectedItem.ToString()!;
            try
            {
                _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    Encoding = Encoding.ASCII,
                    ReadTimeout = 3000,
                    WriteTimeout = 3000,
                    DtrEnable = true,
                    RtsEnable = true
                };
                _serialPort.Open();
                byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                _serialPort.BaseStream.Write(bytes, 0, bytes.Length);
                _serialPort.BaseStream.Flush();
                _serialPort.Close();

                SetStatus($"✓ [COM] 인쇄 완료: {portName} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                    MessageBox.Show(this,
                        $"[COM] {portName} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"COM 인쇄 오류: {ex.Message}");
                _serialPort?.Close();
                return false;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // USB (Windows 스풀러 → RAW)
        // ══════════════════════════════════════════════════════════════════
        private void LoadInstalledPrinters()
        {
            cmbPrinter.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
                cmbPrinter.Items.Add(printer);
            if (cmbPrinter.Items.Count > 0) cmbPrinter.SelectedIndex = 0;
            else cmbPrinter.Items.Add("설치된 프린터 없음");
        }

        private void btnRefreshPrinter_Click(object sender, EventArgs e)
        {
            LoadInstalledPrinters();
            SetStatus("프린터 목록을 새로고침했습니다.", success: true);
        }

        private bool PrintViaUsb(string zpl, string barcodeDisplay, bool showMessageBox = true)
        {
            if (cmbPrinter.SelectedItem?.ToString() == "설치된 프린터 없음"
                || cmbPrinter.SelectedItem == null)
            {
                ShowError("유효한 프린터를 선택해주세요."); return false;
            }

            string printerName = cmbPrinter.SelectedItem.ToString()!;
            try
            {
                SendRawToPrinter(printerName, zpl);
                SetStatus($"✓ [USB] 인쇄 완료: {printerName} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                    MessageBox.Show(this,
                        $"[USB] '{printerName}' 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"USB 인쇄 오류: {ex.Message}");
                return false;
            }
        }

        private static void SendRawToPrinter(string printerName, string zpl)
        {
            if (!OpenPrinter(printerName, out IntPtr hPrinter, IntPtr.Zero))
                throw new InvalidOperationException(
                    $"프린터를 열 수 없습니다: '{printerName}'\n(Win32 오류: {Marshal.GetLastWin32Error()})");
            try
            {
                var di = new DOCINFO { pDocName = "ZPL Print Job", pOutputFile = null, pDataType = "RAW" };
                if (!StartDocPrinter(hPrinter, 1, ref di))
                    throw new InvalidOperationException("인쇄 작업을 시작할 수 없습니다.");
                try
                {
                    if (!StartPagePrinter(hPrinter))
                        throw new InvalidOperationException("페이지 인쇄를 시작할 수 없습니다.");

                    byte[] bytes = Encoding.UTF8.GetBytes(zpl);
                    IntPtr pBytes = Marshal.AllocHGlobal(bytes.Length);
                    try
                    {
                        Marshal.Copy(bytes, 0, pBytes, bytes.Length);
                        if (!WritePrinter(hPrinter, pBytes, bytes.Length, out int written) || written == 0)
                            throw new InvalidOperationException("프린터에 데이터를 쓸 수 없습니다.");
                    }
                    finally { Marshal.FreeHGlobal(pBytes); }
                    EndPagePrinter(hPrinter);
                }
                finally { EndDocPrinter(hPrinter); }
            }
            finally { ClosePrinter(hPrinter); }
        }

        // ══════════════════════════════════════════════════════════════════
        // Ethernet (TCP/IP)
        // ══════════════════════════════════════════════════════════════════
        private bool PrintViaEthernet(string zpl, string barcodeDisplay, bool showMessageBox = true)
        {
            string ip = txtIpAddress.Text.Trim();
            if (string.IsNullOrEmpty(ip)) { ShowError("IP 주소를 입력해주세요."); return false; }

            if (!int.TryParse(txtEthPort.Text.Trim(), out int port) || port < 1 || port > 65535)
            {
                ShowError("올바른 포트 번호를 입력해주세요. (1~65535)"); return false;
            }

            try
            {
                using var client = new TcpClient();
                if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(5)))
                    throw new TimeoutException($"연결 시간 초과 ({ip}:{port})");

                using var stream = client.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                SetStatus($"✓ [Ethernet] 인쇄 완료: {ip}:{port} → {barcodeDisplay}", success: true);
                if (showMessageBox)
                    MessageBox.Show(this,
                        $"[Ethernet] {ip}:{port} 으로 인쇄 명령이 전달되었습니다.\n출력: {barcodeDisplay}",
                        "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Ethernet 인쇄 오류: {ex.Message}");
                return false;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // 인쇄 버튼 (공통 진입점)
        // ══════════════════════════════════════════════════════════════════
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (chkAutoPrint.Checked) PrintSingleWithPreview();
            else PrintScannedList();
        }

        // ── 단일 바코드 미리보기 인쇄 (자동 발행 ON) ──────────────────────
        private void PrintSingleWithPreview()
        {
            if (!ValidateBarcode(out string barcode)) return;

            int copies = (int)nudCopies.Value;
            string zpl = _activeProfile.BuildZpl(barcode, chkShowBarcode.Checked, copies);
            string display = _activeProfile.FormatBarcode(barcode);

            using var preview = new PrintPreviewForm(zpl, display, barcode, _activeProfile, chkShowBarcode.Checked);
            if (preview.ShowDialog() != DialogResult.OK) return;

            DispatchPrint(zpl, display, showMessageBox: true);
        }

        // ── 스캔 목록 일괄 인쇄 (자동 발행 OFF) ──────────────────────────
        private void PrintScannedList()
        {
            if (_scannedBarcodes.Count == 0)
            {
                ShowError("인쇄할 바코드가 없습니다.\n바코드를 먼저 스캔해주세요."); return;
            }

            int total = _scannedBarcodes.Count;
            int copies = (int)nudCopies.Value;
            string copiesMsg = copies > 1 ? $"  (각 {copies}장)" : "";

            if (MessageBox.Show(
                    $"총 {total}개 바코드를 인쇄합니다{copiesMsg}.\n계속하시겠습니까?",
                    "인쇄 확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                != DialogResult.OK) return;

            int successCount = 0;
            foreach (string barcode in _scannedBarcodes)
            {
                string display = _activeProfile.FormatBarcode(barcode);
                string zpl = _activeProfile.BuildZpl(barcode, chkShowBarcode.Checked, copies);

                bool ok = DispatchPrint(zpl, display, showMessageBox: false);
                if (ok)
                {
                    successCount++;
                    System.Threading.Thread.Sleep(_activeProfile.PrintDelayMs);
                }
                else
                {
                    MessageBox.Show(
                        $"{successCount}/{total}개 인쇄 후 오류가 발생했습니다.\n" +
                        $"나머지 {total - successCount}개는 목록에 유지됩니다.",
                        "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _scannedBarcodes.RemoveRange(0, successCount);
                    UpdateScanListUI();
                    return;
                }
            }

            SetStatus($"✓ {total}개 바코드 인쇄 완료", success: true);
            MessageBox.Show($"{total}개 바코드 인쇄가 완료되었습니다.",
                "인쇄 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _scannedBarcodes.Clear();
            UpdateScanListUI();
        }

        /// <summary>
        /// 선택된 연결 방식에 따라 인쇄를 발송한다.
        /// </summary>
        private bool DispatchPrint(string zpl, string barcodeDisplay, bool showMessageBox)
        {
            if (rdoCom.Checked) return PrintViaCom(zpl, barcodeDisplay, showMessageBox);
            if (rdoUsb.Checked) return PrintViaUsb(zpl, barcodeDisplay, showMessageBox);
            return PrintViaEthernet(zpl, barcodeDisplay, showMessageBox);
        }

        // ══════════════════════════════════════════════════════════════════
        // 스캔 목록 관리
        // ══════════════════════════════════════════════════════════════════
        private void AddToScanList(string barcode)
        {
            if (_scannedBarcodes.Contains(barcode))
            {
                SetStatus($"⚠ 중복 바코드 무시됨: {_activeProfile.FormatBarcode(barcode)}", success: false);
                return;
            }
            _scannedBarcodes.Add(barcode);
            UpdateScanListUI();
            SetStatus(
                $"✓ 목록 추가: {_activeProfile.FormatBarcode(barcode)}  (총 {_scannedBarcodes.Count}개)",
                success: true);
        }

        private void UpdateScanListUI()
        {
            lstBarcodes.BeginUpdate();
            lstBarcodes.Items.Clear();
            foreach (string bc in _scannedBarcodes)
                lstBarcodes.Items.Add(_activeProfile.FormatBarcode(bc));
            lstBarcodes.EndUpdate();
            lblScanCount.Text = $"{_scannedBarcodes.Count} 개";
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            if (_scannedBarcodes.Count == 0) return;

            if (MessageBox.Show(
                    $"스캔된 바코드 {_scannedBarcodes.Count}개를 모두 삭제하시겠습니까?",
                    "목록 초기화", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                != DialogResult.OK) return;

            _scannedBarcodes.Clear();
            UpdateScanListUI();
            SetStatus("목록이 초기화되었습니다.", success: true);
        }

        // ══════════════════════════════════════════════════════════════════
        // 바코드 텍스트박스 키 이벤트
        // ══════════════════════════════════════════════════════════════════
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;

            string barcode = txtBarcode.Text.Trim();

            if (chkAutoPrint.Checked)
            {
                PerformDirectPrint();
                return;
            }

            if (!IsValidBarcode(barcode)) { ShowError(BarcodeErrorMessage()); return; }
            AddToScanList(barcode);
            txtBarcode.Clear();
            txtBarcode.Focus();
        }

        private void PerformDirectPrint()
        {
            if (!ValidateBarcode(out string barcode)) return;

            int copies = (int)nudCopies.Value;
            string display = _activeProfile.FormatBarcode(barcode);
            string zpl = _activeProfile.BuildZpl(barcode, chkShowBarcode.Checked, copies);

            bool success = DispatchPrint(zpl, display, showMessageBox: false);
            if (success) { txtBarcode.Clear(); txtBarcode.Focus(); }
        }

        // ── 입력 검증 ─────────────────────────────────────────────────────
        private bool IsValidBarcode(string barcode)
        {
            if (barcode.Length != _activeProfile.BarcodeInputLength) return false;
            foreach (char c in barcode) if (!char.IsDigit(c)) return false;
            return true;
        }

        private string BarcodeErrorMessage() =>
            $"바코드는 {_activeProfile.BarcodeInputLength}자리 숫자여야 합니다.";

        /// <summary>
        /// 텍스트박스 입력값을 검증하고 에러 표시까지 한다.
        /// </summary>
        private bool ValidateBarcode(out string barcode)
        {
            barcode = txtBarcode.Text.Trim();
            if (!IsValidBarcode(barcode))
            {
                ShowError(BarcodeErrorMessage());
                return false;
            }
            return true;
        }

        // ── 상태 표시 / 오류 ─────────────────────────────────────────────
        private void SetStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = success
                ? System.Drawing.Color.FromArgb(0, 180, 120)
                : System.Drawing.Color.FromArgb(220, 60, 60);
        }

        private void ShowError(string message)
        {
            SetStatus($"✗ {message}", success: false);
            MessageBox.Show(message, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
            if (char.IsDigit(e.KeyChar) && txtBarcode.Text.Length >= _activeProfile.BarcodeInputLength)
                e.Handled = true;
        }
    }
}