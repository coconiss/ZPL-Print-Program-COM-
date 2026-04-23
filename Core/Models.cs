using System;
using System.Collections.Generic;

namespace ZplPrinter.Core
{
    public class ProfileField
    {
        public string Key { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string DefaultValue { get; set; } = "";
        public bool UseSegment { get; set; } = false;
        public string SegmentFormat { get; set; } = "";
    }

    public class LabelProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public string Name { get; set; } = "새 라벨";
        public string Description { get; set; } = "";

        public double LabelWidthMm { get; set; } = 94.0;
        public double LabelHeightMm { get; set; } = 26.0;
        public int Dpi { get; set; } = 300;
        public int PrintDelayMs { get; set; } = 100;

        public List<ProfileField> Fields { get; set; } = new List<ProfileField>();

        public string ZplTemplate { get; set; } =
            "^XA\r\n^LH0,0\r\n^JZN\r\n" +
            "^FO200,60\r\n^BY4\r\n^BCN,100,Y,N,N^FD{BARCODE}^FS\r\n" +
            "^FO360,200\r\n^A0,40,40^FD{ITEM_NAME}^FS\r\n" +
            "^RFW,E,1,2,1^FD3000^FS\r\n^RFW,E,2,12,1^FD{RFID_EPC}^FS\r\n" +
            "^RFR,E,^FN1^FS\r\n^PQ{COPIES}\r\n^XZ";

        public string ZplTemplateEncodingOnly { get; set; } =
            "^XA\r\n^LH0,0\r\n^JZN\r\n" +
            "^RFW,E,1,2,1^FD3000^FS\r\n^RFW,E,2,12,1^FD{RFID_EPC}^FS\r\n" +
            "^RFR,E,^FN1^FS\r\n^PQ{COPIES}\r\n^XZ";

        public string BuildZpl(Dictionary<string, string> values, int copies, bool showBarcode)
        {
            string result = showBarcode ? ZplTemplate : ZplTemplateEncodingOnly;

            result = result.Replace("{COPIES}", copies.ToString(), StringComparison.OrdinalIgnoreCase);
            result = result.Replace("{WIDTH_DOTS}", Math.Round(LabelWidthMm / 25.4 * Dpi).ToString(), StringComparison.OrdinalIgnoreCase);
            result = result.Replace("{HEIGHT_DOTS}", Math.Round(LabelHeightMm / 25.4 * Dpi).ToString(), StringComparison.OrdinalIgnoreCase);

            var computedValues = new Dictionary<string, string>(values);

            foreach (var field in Fields)
            {
                string rawValue = values.ContainsKey(field.Key) ? values[field.Key] : "";

                // 참조 변수 맵핑 처리
                if (computedValues.ContainsKey(rawValue))
                {
                    rawValue = computedValues[rawValue];
                }

                // 세그먼트 적용
                if (field.UseSegment && !string.IsNullOrEmpty(field.SegmentFormat))
                {
                    rawValue = ApplySegment(rawValue, field.SegmentFormat);
                }

                computedValues[field.Key] = rawValue;
            }

            foreach (var kvp in computedValues)
            {
                result = result.Replace($"{{{kvp.Key}}}", kvp.Value, StringComparison.OrdinalIgnoreCase);
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(result, @"\^PQ", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                result = System.Text.RegularExpressions.Regex.Replace(result, @"\^XZ", $"^PQ{copies}\r\n^XZ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return result;
        }

        // 공백 대신 '-'를 사용하여 포맷팅하고, 폼에서도 쓸 수 있도록 public static 적용
        public static string ApplySegment(string input, string format)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // 중복 분할을 막기 위해 기존 하이픈과 공백 제거
            input = input.Replace("-", "").Replace(" ", "");

            var parts = format.Split(',');
            var segments = new List<string>();
            int currentIndex = 0;

            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int length) && length > 0)
                {
                    if (currentIndex >= input.Length) break;
                    int currentLen = Math.Min(length, input.Length - currentIndex);
                    segments.Add(input.Substring(currentIndex, currentLen));
                    currentIndex += currentLen;
                }
            }

            if (currentIndex < input.Length)
            {
                segments.Add(input.Substring(currentIndex));
            }

            // 공백 대신 '-' 로 연결
            return string.Join("-", segments);
        }
    }
}