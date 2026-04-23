using System;
using System.IO;
using System.Text.Json;

namespace ZplPrinter.Core
{
    // 프로그램 환경 설정 데이터 모델
    public class AppSettings
    {
        public int LastConnType { get; set; } = 0;
        public string LastComPort { get; set; } = "";
        public string LastBaudRate { get; set; } = "9600";
        public string LastDataBits { get; set; } = "8";
        public string LastParity { get; set; } = "None";
        public string LastStopBits { get; set; } = "One";
        public string LastUsbPrinter { get; set; } = "";
        public string LastIp { get; set; } = "192.168.1.100";
        public string LastTcpPort { get; set; } = "9100";
        public string LastProfileName { get; set; } = "";
        public bool ShowBarcode { get; set; } = true;
        public bool AutoPrint { get; set; } = false;
        public int DefaultCopies { get; set; } = 1;
    }

    // 설정 파일 저장/로드 매니저
    public static class SettingsManager
    {
        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { /* 파일이 없거나 깨졌을 경우 무시하고 기본값 반환 */ }
            return new AppSettings();
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, opts));
            }
            catch { /* 권한 문제 등으로 저장 실패 시 무시 */ }
        }
    }
}