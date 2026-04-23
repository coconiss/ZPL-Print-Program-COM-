using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ZplPrinter.Core
{
    public static class ProfileManager
    {
        private static string ProfilesDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles");
        private static readonly JsonSerializerOptions _jsonOpts = new() { WriteIndented = true };

        public static List<LabelProfile> LoadAll()
        {
            if (!Directory.Exists(ProfilesDir)) Directory.CreateDirectory(ProfilesDir);
            var list = new List<LabelProfile>();

            foreach (string file in Directory.GetFiles(ProfilesDir, "*.json"))
            {
                try
                {
                    var p = JsonSerializer.Deserialize<LabelProfile>(File.ReadAllText(file, Encoding.UTF8));
                    if (p != null) list.Add(p);
                }
                catch { /* 손상된 파일 스킵 */ }
            }

            if (list.Count == 0)
            {
                var def = CreateDefaultProfile();
                Save(def);
                list.Add(def);
            }

            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        public static void Save(LabelProfile profile)
        {
            if (!Directory.Exists(ProfilesDir)) Directory.CreateDirectory(ProfilesDir);
            File.WriteAllText(Path.Combine(ProfilesDir, profile.Id + ".json"), JsonSerializer.Serialize(profile, _jsonOpts), Encoding.UTF8);
        }

        public static void Delete(LabelProfile profile)
        {
            string path = Path.Combine(ProfilesDir, profile.Id + ".json");
            if (File.Exists(path)) File.Delete(path);
        }

        public static LabelProfile DeepCopy(LabelProfile source) =>
            JsonSerializer.Deserialize<LabelProfile>(JsonSerializer.Serialize(source, _jsonOpts))!;

        private static LabelProfile CreateDefaultProfile()
        {
            var p = new LabelProfile
            {
                Id = "default01",
                Name = "RFID 라벨 (예시)",
                Description = "바코드, RFID EPC 데이터를 매핑하여 출력합니다."
            };
            p.Fields.Add(new ProfileField { Key = "BARCODE", DisplayName = "바코드번호", DefaultValue = "123456789012" });
            p.Fields.Add(new ProfileField { Key = "ITEM_NAME", DisplayName = "상품명", DefaultValue = "검정 티셔츠" });
            p.Fields.Add(new ProfileField { Key = "RFID_EPC", DisplayName = "RFID EPC", DefaultValue = "301400000000000000000000" });
            return p;
        }
    }
}