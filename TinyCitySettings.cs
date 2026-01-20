using System.Runtime.InteropServices;
using System.Text.Json;

namespace TinyCity
{
    public class TinyCitySettings
    {
        public List<string> BrowserBookmarkPaths { get; set; } = new List<string>();

        public List<string> MarkdownFiles { get; set; } = new List<string>();

        public List<string> HtmlBookmarksFiles { get; set; } = new List<string>();

        public string ApplicationConfigDirectory { get; set; }

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
            string homePath;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                homePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                homePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                homePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
            }
            else
            {
                throw new NotSupportedException("Unsupported platform.");
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
