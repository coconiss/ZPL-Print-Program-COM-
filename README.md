# ZPL Barcode Printer — WinForms (.NET 8.0)

COM 시리얼 포트를 통해 ZPL 커멘드를 전송하는 바코드 라벨 프린터 제어 프로그램입니다.

---

## 📁 프로젝트 구조

```
ZplPrinter/
├── ZplPrinter.sln
└── ZplPrinter/
    ├── ZplPrinter.csproj
    ├── Program.cs              ← 진입점
    ├── Form1.cs                ← 메인 화면 로직
    ├── Form1.Designer.cs       ← 메인 화면 UI
    ├── PrintPreviewForm.cs     ← 인쇄 전 미리보기 창
    └── PrintResultForm.cs      ← 인쇄 결과 확인 창
```

---

## ▶ 빌드 및 실행

```bash
cd ZplPrinter
dotnet build
dotnet run --project ZplPrinter/ZplPrinter.csproj
```

또는 Visual Studio 2022에서 `ZplPrinter.sln` 을 열어 실행.

---

## 🔧 주요 기능

| 기능 | 설명 |
|------|------|
| COM 포트 선택 | 시스템에서 사용 가능한 포트 자동 감지, 새로고침 지원 |
| 바코드 입력 | 12자리 숫자 전용 (숫자 외 입력 차단) |
| 표시 포맷 자동 변환 | `252190021426` → `252-19-0021426` |
| 라벨 크기 설정 | 가로/세로 inch 단위 입력 (Double) |
| DPI 고정 | 300 DPI (dots = inch × 300) |
| Human Readable 토글 | 체크박스로 바코드 하단 숫자 출력 여부 선택 |
| 인쇄 미리보기 | 전송 전 ZPL 내용 및 라벨 시각 미리보기 |
| 결과 확인 창 | 정상/재인쇄/실패 3가지 응답 + 점검 가이드 |

---

## 📄 생성되는 ZPL 커멘드 예시

입력값: `252190021426`, 라벨 4.0×2.0 inch, Human Readable ON

```zpl
^XA
^PW1200
^LL600
^FO400,132
^A0,50,50^FD252-19-0021426^FS
^FO200,168
^BY4
^BCN,100,Y,N,N^FD252190021426^FS
^RFW,H,1,2,1^FD3000^FS
^RFW,A,2,12,1^FD252190021426^FS
^RFR,A,^FN1^FS
^FH_^HV1,,EPC-Ascii  DATA:[,]_0D_0A^FS
^PQ1
^XZ
```

---

## ⚙ COM 포트 통신 파라미터

기본값 (변경 필요 시 `Form1.cs` 의 `btnPrint_Click` 수정):

```
BaudRate  : 9600
DataBits  : 8
Parity    : None
StopBits  : One
```

---

## 🔢 바코드 포맷 규칙

| 입력 (12자리) | 출력 표기 |
|-------------|---------|
| `252190021426` | `252-19-0021426` |
| `[AAA][BB][CCCCCCC]` | `AAA-BB-CCCCCCC` |

- 앞 3자리 + `-` + 다음 2자리 + `-` + 나머지 7자리
