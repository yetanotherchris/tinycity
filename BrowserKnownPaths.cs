using System.Runtime.InteropServices;

namespace TinyCity
{
    public class BrowserKnownPaths
    {
        public static string ChromeBookmarksPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Google", "Chrome", "User Data", "Default", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Google", "Chrome", "Default", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "google-chrome", "Default", "Bookmarks");
                }
                else
                {
                    throw new NotSupportedException("Unsupported platform.");
                }
            }
        }

        public static string EdgeBookmarksPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Microsoft", "Edge", "User Data", "Default", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    throw new NotImplementedException("MacOS path not implemented for Edge.");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    throw new NotImplementedException("Linux path not implemented for Edge.");
                }
                else
                {
                    throw new NotSupportedException("Unsupported platform.");
                }
            }
        }

        public static string BraveBookmarksPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "BraveSoftware", "Brave-Browser", "User Data", "Default", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "BraveSoftware", "Brave-Browser", "Default", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "BraveSoftware", "Brave-Browser", "Default", "Bookmarks");
                }
                else
                {
                    throw new NotSupportedException("Unsupported platform.");
                }
            }
        }

        public static string OperaBookmarksPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "Opera Software", "Opera Stable", "User Data", "Default", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "com.operasoftware.Opera", "Bookmarks");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "opera");
                }
                else
                {
                    throw new NotSupportedException("Unsupported platform.");
                }
            }
        }
    }
}
