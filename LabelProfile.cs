using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZplPrinter
{
    /// <summary>
    /// 라벨 한 종류의 모든 설정을 담는 모델.
    ///
    /// 기본 ZPL 토큰
    ///   {BARCODE}         — 입력 원본 숫자열
    ///   {BARCODE_DISPLAY} — 세그먼트 포맷 변환된 표시 문자열
    ///   {COPIES}          — 발행 매수
    ///   {WIDTH_DOTS}      — 라벨 가로 dots (mm × DPI ÷ 25.4)
    ///   {HEIGHT_DOTS}     — 라벨 세로 dots
    ///
    /// 추가 토큰은 BuildZpl의 extraTokens 매개변수로 전달한다.
    /// {COPIES} / ^PQ 미포함 시 ^XZ 앞에 자동 삽입한다.
    /// </summary>
    public class LabelProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public string Name { get; set; } = "새 라벨";
        public string Description { get; set; } = "";

        // ── 라벨 물리 사양 ─────────────────────────────────────────────
        public double LabelWidthMm { get; set; } = 94.0;
        public double LabelHeightMm { get; set; } = 26.0;
        public int Dpi { get; set; } = 300;

        // ── 바코드 입력 규칙 ───────────────────────────────────────────
        public int BarcodeInputLength { get; set; } = 12;

        /// <summary>
        /// 표시 포맷 세그먼트. [3,2,7] → "AAA-BB-CCCCCCC"
        /// 비어 있으면 raw 그대로 반환한다.
        /// </summary>
        public int[] BarcodeSegments { get; set; } = new[] { 3, 2, 7 };

        // ── ZPL 템플릿 ─────────────────────────────────────────────────
        public string ZplTemplate { get; set; } = DefaultZplWithBarcode;
        public string ZplTemplateEncodingOnly { get; set; } = DefaultZplEncodingOnly;

        // ── 인쇄 딜레이 ────────────────────────────────────────────────
        public int PrintDelayMs { get; set; } = 1000;

        // ── 내장 기본 템플릿 ───────────────────────────────────────────
        public const string DefaultZplWithBarcode =
            "^XA\r\n^LH0,0\r\n^JZN\r\n" +
            "^FO360,180\r\n^A0,50,50^FD{BARCODE_DISPLAY}^FS\r\n" +
            "^FO200,60\r\n^BY4\r\n^BCN,100,N,N,N^FD{BARCODE}^FS\r\n" +
            "^RFW,H,1,2,1^FD3000^FS\r\n^RFW,A,2,12,1^FD{BARCODE}^FS\r\n" +
            "^RFR,A,^FN1^FS\r\n^PQ{COPIES}\r\n^XZ";

        public const string DefaultZplEncodingOnly =
            "^XA\r\n^LH0,0\r\n^JZN\r\n" +
            "^RFW,H,1,2,1^FD3000^FS\r\n^RFW,A,2,12,1^FD{BARCODE}^FS\r\n" +
            "^RFR,A,^FN1^FS\r\n^PQ{COPIES}\r\n^XZ";

        // ── 공개 API ──────────────────────────────────────────────────

        /// <summary>
        /// BarcodeSegments에 따라 바코드를 표시 포맷으로 변환한다.
        /// </summary>
        public string FormatBarcode(string raw)
        {
            if (BarcodeSegments == null || BarcodeSegments.Length == 0)
                return raw;

            var sb = new StringBuilder();
            int pos = 0;
            for (int i = 0; i < BarcodeSegments.Length && pos < raw.Length; i++)
            {
                int len = Math.Min(BarcodeSegments[i], raw.Length - pos);
                if (i > 0) sb.Append('-');
                sb.Append(raw, pos, len);
                pos += len;
            }
            if (pos < raw.Length) sb.Append(raw, pos, raw.Length - pos);
            return sb.ToString();
        }

        /// <summary>
        /// ZPL 커맨드 문자열을 생성한다.
        /// <para>
        /// 토큰 치환 순서: 기본 토큰 → extraTokens.
        /// 치환 후 ^PQ 커맨드가 없으면 ^XZ 직전에 자동 삽입한다.
        /// </para>
        /// </summary>
        /// <param name="extraTokens">추가 토큰 딕셔너리. 키는 "{TOKEN}" 형식.</param>
        public string BuildZpl(string barcode, bool showBarcode, int copies,
            Dictionary<string, string>? extraTokens = null)
        {
            string template = showBarcode ? ZplTemplate : ZplTemplateEncodingOnly;

            // 기본 토큰 구성
            var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["{BARCODE}"] = barcode,
                ["{BARCODE_DISPLAY}"] = FormatBarcode(barcode),
                ["{COPIES}"] = copies.ToString(),
                ["{WIDTH_DOTS}"] = ((int)Math.Round(LabelWidthMm / 25.4 * Dpi)).ToString(),
                ["{HEIGHT_DOTS}"] = ((int)Math.Round(LabelHeightMm / 25.4 * Dpi)).ToString(),
            };

            // 추가 토큰 병합 (동일 키는 extra가 덮어씀)
            if (extraTokens != null)
                foreach (var kv in extraTokens)
                    tokens[kv.Key] = kv.Value;

            // 토큰 치환
            string result = template;
            foreach (var kv in tokens)
                result = result.Replace(kv.Key, kv.Value, StringComparison.OrdinalIgnoreCase);

            // ^PQ가 없으면 ^XZ 직전에 삽입
            if (!Regex.IsMatch(result, @"\^PQ", RegexOptions.IgnoreCase))
                result = Regex.Replace(result, @"\^XZ",
                    $"^PQ{copies}\r\n^XZ", RegexOptions.IgnoreCase);

            return result;
        }

        /// <summary>세그먼트 배열 → 편집 문자열. 예) "3,2,7"</summary>
        public string SegmentsToString() =>
            BarcodeSegments != null ? string.Join(",", BarcodeSegments) : "";

        /// <summary>"3,2,7" → BarcodeSegments 설정</summary>
        public void SetSegmentsFromString(string s)
        {
            var list = new List<int>();
            foreach (var part in s.Split(','))
                if (int.TryParse(part.Trim(), out int v) && v > 0)
                    list.Add(v);
            BarcodeSegments = list.Count > 0 ? list.ToArray() : Array.Empty<int>();
        }
    }
}