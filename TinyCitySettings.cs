using System.Text.Json;

namespace TinyCity
{
    public class TinyCitySettings
    {
        public List<string> BrowserBookmarkPaths { get; set; } = new List<string>();

        public List<string> MarkdownFiles { get; set; } = new List<string>();

        public List<string> HtmlBookmarksFiles { get; set; } = new List<string>();

        public string ApplicationConfigDirectory { get; set; }

        public string S3AccessKey { get; set; } = "";

        public string S3SecretKey { get; set; } = "";

        public string S3Endpoint { get; set; } = "";

        public string S3Bucket { get; set; } = "";

        public string S3KeyPrefix { get; set; } = "";

        public static TinyCitySettings Load()
        {
            var configFilePath = GetConfigFilePath();
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize(json, TinyCityJsonContext.Default.TinyCitySettings) ?? new TinyCitySettings();
            }
            else
            {
                var settings = new TinyCitySettings();
                settings.ApplicationConfigDirectory = GetApplicationDirectory();
                Save(settings);

                return settings;
            }
        }

        public static void Save(TinyCitySettings settings)
        {
            var configFilePath = GetConfigFilePath();
            string json = JsonSerializer.Serialize(settings, TinyCityJsonContext.Default.TinyCitySettings);
            File.WriteAllText(configFilePath, json);
        }

        public static string GetConfigFilePath()
        {
            string homeDirectory = GetApplicationDirectory();
            return Path.Combine(homeDirectory, "config.json");
        }

        private static string GetApplicationDirectory()
        {
            string homePath = "";

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                homePath = Path.Combine(homePath, ".config");
            }
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // todo
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                homePath = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            }

            if (string.IsNullOrEmpty(homePath))
            {
                throw new Exception("Unable to determine home directory.");
            }

            string applicationPath = Path.Combine(homePath, "tinycity");
            if (!Directory.Exists(applicationPath))
            {
                Directory.CreateDirectory(applicationPath);
            }

            return applicationPath;
        }
    }
}
