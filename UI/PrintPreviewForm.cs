using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZplPrinter.Core;

namespace ZplPrinter.UI
{
    public partial class PrintPreviewForm : Form
    {
        private string _zpl;
        private LabelProfile _profile;

        public PrintPreviewForm(string generatedZpl, Dictionary<string, string> values, LabelProfile profile)
        {
            _zpl = generatedZpl;
            _profile = profile;

            InitializeComponent();
            foreach (var kvp in values) dgvInfo.Rows.Add(kvp.Key, kvp.Value);

            // 폼이 로드될 때 비동기로 이미지 렌더링 시작
            this.Load += PrintPreviewForm_Load;
        }

        private async void PrintPreviewForm_Load(object? sender, EventArgs e)
        {
            await LoadPreviewImageAsync();
        }

        private async Task LoadPreviewImageAsync()
        {
            try
            {
                lblLoading.Visible = true;
                picPreview.Visible = false;

                // Labelary API는 dpmm(밀리미터당 도트수)와 인치(inch) 단위를 사용합니다.
                int dpmm = _profile.Dpi == 203 ? 8 : (_profile.Dpi == 600 ? 24 : 12);
                double wInch = Math.Round(_profile.LabelWidthMm / 25.4, 2);
                double hInch = Math.Round(_profile.LabelHeightMm / 25.4, 2);

                string url = $"http://api.labelary.com/v1/printers/{dpmm}dpmm/labels/{wInch}x{hInch}/0/";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/png"));

                // ZPL 문자열을 POST 방식으로 전송하여 렌더링된 PNG 이미지 스트림을 받아옴
                var content = new StringContent(_zpl, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    picPreview.Image = Image.FromStream(stream);
                    picPreview.Visible = true;
                }
                else
                {
                    lblLoading.Text = "미리보기 렌더링 실패 (API 오류)\n프린터 출력은 정상적으로 가능합니다.";
                    lblLoading.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                lblLoading.Text = $"렌더링 오류 (인터넷 연결 확인)\n프린터 출력은 가능합니다.\n\n{ex.Message}";
                lblLoading.ForeColor = Color.IndianRed;
            }
            finally
            {
                if (picPreview.Visible) lblLoading.Visible = false;
            }
        }

        private void btnOk_Click(object sender, EventArgs e) => DialogResult = DialogResult.OK;
        private void btnCancel_Click(object sender, EventArgs e) => DialogResult = DialogResult.Cancel;
    }
}