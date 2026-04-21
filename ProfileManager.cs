using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace ZplPrinter
{
    /// <summary>
    /// 프로필 파일을 AppData\Roaming\…\profiles\ 폴더에서 관리한다.
    /// </summary>
    public static class ProfileManager
    {
        private static string ProfilesDir =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");

        private static readonly JsonSerializerOptions _json =
            new JsonSerializerOptions { WriteIndented = true };

        // ── 공개 API ─────────────────────────────────────────────────────

        public static List<LabelProfile> LoadAll()
        {
            EnsureDir();
            var list = new List<LabelProfile>();

            foreach (string file in Directory.GetFiles(ProfilesDir, "*.json"))
            {
                try
                {
                    var p = JsonSerializer.Deserialize<LabelProfile>(
                        File.ReadAllText(file, Encoding.UTF8));
                    if (p != null) list.Add(p);
                }
                catch { /* 손상된 파일 무시 */ }
            }

            if (list.Count == 0)
            {
                var def = CreateDefaultProfile();
                Save(def);
                list.Add(def);
            }

            list.Sort((a, b) =>
                string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        public static void Save(LabelProfile profile)
        {
            EnsureDir();
            File.WriteAllText(
                Path.Combine(ProfilesDir, profile.Id + ".json"),
                JsonSerializer.Serialize(profile, _json),
                Encoding.UTF8);
        }

        public static void Delete(LabelProfile profile)
        {
            string path = Path.Combine(ProfilesDir, profile.Id + ".json");
            if (File.Exists(path)) File.Delete(path);
        }

        /// <summary>
        /// 프로필을 JSON 직렬화/역직렬화로 깊은 복사한다.
        /// </summary>
        public static LabelProfile DeepCopy(LabelProfile source) =>
            JsonSerializer.Deserialize<LabelProfile>(
                JsonSerializer.Serialize(source, _json))!;

        // ── 내부 ─────────────────────────────────────────────────────────

        private static void EnsureDir()
        {
            if (!Directory.Exists(ProfilesDir))
                Directory.CreateDirectory(ProfilesDir);
        }

        public static LabelProfile CreateDefaultProfile() => new LabelProfile
        {
            Id = "default01",
            Name = "기본 라벨 (94×26mm, RFID)",
            Description = "다이소 표준 RFID 라벨 · 300 DPI · 12자리 바코드",
            LabelWidthMm = 94,
            LabelHeightMm = 26,
            Dpi = 300,
            BarcodeInputLength = 12,
            BarcodeSegments = new[] { 3, 2, 7 },
            ZplTemplate = LabelProfile.DefaultZplWithBarcode,
            ZplTemplateEncodingOnly = LabelProfile.DefaultZplEncodingOnly,
            PrintDelayMs = 1000
        };
    }
}