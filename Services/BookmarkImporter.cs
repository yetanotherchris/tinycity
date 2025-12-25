using System.Text.Json;
using TinyCity.Model;

namespace TinyCity.Services
{
    public class BookmarkImporter
    {
        public async Task ImportFromChromiumJsonAsync(string jsonContent, string targetPath, FileBackupService backup)
        {
            var bookmarksFile = JsonSerializer.Deserialize(jsonContent, TinyCityJsonContext.Default.BookmarksFile);
            if (bookmarksFile == null)
            {
                throw new InvalidOperationException("Failed to parse Chromium JSON format.");
            }

            backup.CreateBackup(targetPath);
            await File.WriteAllTextAsync(targetPath, jsonContent);
        }

        public async Task ImportFromMarkdownAsync(string mdContent, string targetPath, FileBackupService backup)
        {
            if (string.IsNullOrWhiteSpace(mdContent))
            {
                throw new ArgumentException("Markdown content is empty.");
            }

            backup.CreateBackup(targetPath);
            await File.WriteAllTextAsync(targetPath, mdContent);
        }

        public async Task ImportFromHtmlAsync(string htmlContent, string targetPath, FileBackupService backup)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("HTML content is empty.");
            }

            backup.CreateBackup(targetPath);
            await File.WriteAllTextAsync(targetPath, htmlContent);
        }
    }
}
