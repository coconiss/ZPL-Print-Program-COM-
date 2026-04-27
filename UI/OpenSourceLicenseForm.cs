using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZplPrinter.UI
{
    public partial class OpenSourceLicenseForm : Form
    {
        private RichTextBox rtbLicenseInfo;

        public OpenSourceLicenseForm()
        {
            InitializeComponent();
            LoadLicenseData();
        }

        private void LoadLicenseData()
        {
            string licenseText = @"본 프로그램(ZPL Universal RFID/Barcode Printer PRO)은 다음의 오픈소스 소프트웨어 및 외부 서비스를 사용하고 있습니다.

====================================================================
1. .NET Runtime & System.IO.Ports
====================================================================
- 제공자 (Provider): .NET Foundation and Contributors
- 라이선스 (License): MIT License
- 관련 링크 (Link): https://github.com/dotnet/runtime

The MIT License (MIT)

Copyright (c) .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

====================================================================
2. Labelary API (Print Preview Web Service)
====================================================================
- 제공자 (Provider): Labelary
- 용도: ZPL 코드를 이미지(PNG)로 변환하는 '인쇄 미리보기' 기능 렌더링
- 관련 링크 (Link): http://labelary.com/

설명: 
Labelary는 ZPL 템플릿 검증 및 렌더링을 위한 무료 온라인 API 서비스입니다.
본 프로그램은 인쇄 미리보기 화면(PrintPreviewForm)에서 해당 API를 호출하여 시각화된 이미지를 수신합니다. 
프로그램은 Labelary 서비스의 API 사용 정책을 존중하며 상업적 오프라인 배포용으로 API 자체를 재판매하거나 남용하지 않습니다.
====================================================================
";
            rtbLicenseInfo.Text = licenseText;
        }
    }
}