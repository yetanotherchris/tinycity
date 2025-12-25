using Spectre.Console;
using System.CommandLine;
using TinyCity.Commands.Settings;
using TinyCity.Services;

namespace TinyCity.Commands
{
    public class ExportCommandHandler : BaseCommandHandler<ExportCommandSettings>
    {
        private readonly TinyCitySettings _settings;

        public ExportCommandHandler(TinyCitySettings settings)
        {
            _settings = settings;
        }

        public override async Task<int> ExecuteAsync(ExportCommandSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Type))
            {
                AnsiConsole.MarkupLine("[red]Option --type is required. Valid values: remote, local[/]");
                return 1;
            }

            if (string.IsNullOrEmpty(settings.Source))
            {
                AnsiConsole.MarkupLine("[red]Option --source is required. Valid values: chrome, brave, edge, opera, markdown, html, all[/]");
                return 1;
            }

            string type = settings.Type.ToLowerInvariant();
            string source = settings.Source.ToLowerInvariant();

            if (type != "remote" && type != "local")
            {
                AnsiConsole.MarkupLine($"[red]Invalid type '{settings.Type}'. Valid values: remote, local[/]");
                return 1;
            }

            if (type == "remote")
            {
                var s3Settings = ResolveS3Settings(settings);
                ValidateS3Settings(s3Settings);

                if (settings.SaveCredentials)
                {
                    SaveS3Credentials(s3Settings);
                }

                if (source == "all")
                {
                    await ExportAllToS3(s3Settings);
                }
                else
                {
                    await ExportSingleToS3(source, s3Settings);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(settings.Directory))
                {
                    AnsiConsole.MarkupLine("[red]Option --directory is required for local export[/]");
                    return 1;
                }

                if (!Directory.Exists(settings.Directory))
                {
                    Directory.CreateDirectory(settings.Directory);
                }

                if (source == "all")
                {
                    await ExportAllToLocal(settings.Directory);
                }
                else
                {
                    await ExportSingleToLocal(source, settings.Directory);
                }
            }

            return 0;
        }

        private S3Settings ResolveS3Settings(ExportCommandSettings settings)
        {
            return new S3Settings
            {
                Endpoint = settings.S3Endpoint ?? _settings.S3Endpoint,
                AccessKey = settings.S3AccessKey ?? _settings.S3AccessKey,
                SecretKey = settings.S3SecretKey ?? _settings.S3SecretKey,
                Bucket = settings.Bucket ?? _settings.S3Bucket,
                KeyPrefix = _settings.S3KeyPrefix
            };
        }

        private void ValidateS3Settings(S3Settings s3)
        {
            if (string.IsNullOrEmpty(s3.Endpoint))
                throw new ArgumentException("S3 endpoint not configured. Use --s3-endpoint or configure it.");
            if (string.IsNullOrEmpty(s3.AccessKey))
                throw new ArgumentException("S3 access key not configured. Use --s3-access-key or configure it.");
            if (string.IsNullOrEmpty(s3.SecretKey))
                throw new ArgumentException("S3 secret key not configured. Use --s3-secret-key or configure it.");
            if (string.IsNullOrEmpty(s3.Bucket))
                throw new ArgumentException("S3 bucket not configured. Use --bucket or configure it.");
        }

        private void SaveS3Credentials(S3Settings s3Settings)
        {
            _settings.S3Endpoint = s3Settings.Endpoint;
            _settings.S3AccessKey = s3Settings.AccessKey;
            _settings.S3SecretKey = s3Settings.SecretKey;
            _settings.S3Bucket = s3Settings.Bucket;
            TinyCitySettings.Save(_settings);

            AnsiConsole.MarkupLine("[yellow]S3 credentials saved to config[/]");
        }

        private async Task ExportAllToS3(S3Settings s3Settings)
        {
            var exportedFiles = new List<string>();

            foreach (var browserPath in _settings.BrowserBookmarkPaths)
            {
                if (File.Exists(browserPath))
                {
                    string browserType = GetBrowserTypeFromPath(browserPath);
                    string filename = $"{browserType}.json";
                    await ExportFileToS3(browserPath, filename, s3Settings);
                    exportedFiles.Add(filename);
                }
            }

            foreach (var mdFile in _settings.MarkdownFiles)
            {
                if (File.Exists(mdFile))
                {
                    string filename = Path.GetFileName(mdFile);
                    await ExportFileToS3(mdFile, filename, s3Settings);
                    exportedFiles.Add(filename);
                }
            }

            foreach (var htmlFile in _settings.HtmlBookmarksFiles)
            {
                if (File.Exists(htmlFile))
                {
                    string filename = Path.GetFileName(htmlFile);
                    await ExportFileToS3(htmlFile, filename, s3Settings);
                    exportedFiles.Add(filename);
                }
            }

            if (exportedFiles.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No bookmark files found to export[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold green]Exported {exportedFiles.Count} file(s) to S3: {string.Join(", ", exportedFiles)}[/]");
            }
        }

        private async Task ExportSingleToS3(string source, S3Settings s3Settings)
        {
            string? sourcePath = GetSourcePath(source);
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new InvalidOperationException($"No {source} bookmark file configured. Run 'tinycity config' to configure.");
            }

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"Bookmark file not found: {sourcePath}");
            }

            string filename = GetFilenameForSource(source, sourcePath);
            await ExportFileToS3(sourcePath, filename, s3Settings);

            AnsiConsole.MarkupLine($"[bold green]Exported {filename} to S3 bucket '{s3Settings.Bucket}'[/]");
        }

        private async Task ExportFileToS3(string sourcePath, string filename, S3Settings s3Settings)
        {
            var s3Service = new S3Service(s3Settings.Endpoint, s3Settings.AccessKey, s3Settings.SecretKey);
            string objectKey = string.IsNullOrEmpty(s3Settings.KeyPrefix)
                ? filename
                : $"{s3Settings.KeyPrefix}/{filename}";

            await s3Service.UploadFileAsync(s3Settings.Bucket, objectKey, sourcePath);
        }

        private async Task ExportAllToLocal(string directory)
        {
            var exportedFiles = new List<string>();

            foreach (var browserPath in _settings.BrowserBookmarkPaths)
            {
                if (File.Exists(browserPath))
                {
                    string browserType = GetBrowserTypeFromPath(browserPath);
                    string filename = $"{browserType}.json";
                    string destPath = Path.Combine(directory, filename);
                    File.Copy(browserPath, destPath, overwrite: true);
                    exportedFiles.Add(filename);
                }
            }

            foreach (var mdFile in _settings.MarkdownFiles)
            {
                if (File.Exists(mdFile))
                {
                    string filename = Path.GetFileName(mdFile);
                    string destPath = Path.Combine(directory, filename);
                    File.Copy(mdFile, destPath, overwrite: true);
                    exportedFiles.Add(filename);
                }
            }

            foreach (var htmlFile in _settings.HtmlBookmarksFiles)
            {
                if (File.Exists(htmlFile))
                {
                    string filename = Path.GetFileName(htmlFile);
                    string destPath = Path.Combine(directory, filename);
                    File.Copy(htmlFile, destPath, overwrite: true);
                    exportedFiles.Add(filename);
                }
            }

            if (exportedFiles.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No bookmark files found to export[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold green]Exported {exportedFiles.Count} file(s) to {directory}: {string.Join(", ", exportedFiles)}[/]");
            }
        }

        private async Task ExportSingleToLocal(string source, string directory)
        {
            string? sourcePath = GetSourcePath(source);
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new InvalidOperationException($"No {source} bookmark file configured. Run 'tinycity config' to configure.");
            }

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"Bookmark file not found: {sourcePath}");
            }

            string filename = GetFilenameForSource(source, sourcePath);
            string destPath = Path.Combine(directory, filename);
            File.Copy(sourcePath, destPath, overwrite: true);

            AnsiConsole.MarkupLine($"[bold green]Exported {filename} to {directory}[/]");
        }

        private string? GetSourcePath(string source)
        {
            string sourceLower = source.ToLowerInvariant();

            if (sourceLower == "chrome" || sourceLower == "brave" || sourceLower == "edge" || sourceLower == "opera")
            {
                foreach (var browserPath in _settings.BrowserBookmarkPaths)
                {
                    string browserType = GetBrowserTypeFromPath(browserPath);
                    if (browserType.Equals(sourceLower, StringComparison.OrdinalIgnoreCase))
                    {
                        return browserPath;
                    }
                }
                return null;
            }
            else if (sourceLower == "markdown")
            {
                return _settings.MarkdownFiles.Count > 0 ? _settings.MarkdownFiles[0] : null;
            }
            else if (sourceLower == "html")
            {
                return _settings.HtmlBookmarksFiles.Count > 0 ? _settings.HtmlBookmarksFiles[0] : null;
            }

            return null;
        }

        private string GetBrowserTypeFromPath(string path)
        {
            if (path.Contains("Chrome", StringComparison.OrdinalIgnoreCase)) return "chrome";
            if (path.Contains("Brave", StringComparison.OrdinalIgnoreCase)) return "brave";
            if (path.Contains("Edge", StringComparison.OrdinalIgnoreCase)) return "edge";
            if (path.Contains("Opera", StringComparison.OrdinalIgnoreCase)) return "opera";
            return "chrome";
        }

        private string GetFilenameForSource(string source, string sourcePath)
        {
            return source.ToLowerInvariant() switch
            {
                "chrome" => "chrome.json",
                "brave" => "brave.json",
                "edge" => "edge.json",
                "opera" => "opera.json",
                "markdown" => Path.GetFileName(sourcePath),
                "html" => Path.GetFileName(sourcePath),
                _ => Path.GetFileName(sourcePath)
            };
        }

        public override Command CreateCommand(ExtraArgumentHandler extraArgumentHandler)
        {
            var command = new Command("export", "Export bookmarks to S3 or local filesystem.");

            var settingsBinder = new ExportCommandSettings();
            settingsBinder.AddOptionsToCommand(command);

            command.SetHandler(async (ExportCommandSettings settings) =>
            {
                try
                {
                    extraArgumentHandler.SetShowExtraInfo(settings.Extra);
                    await ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
                    if (settings.Extra)
                    {
                        AnsiConsole.MarkupLine(Markup.Escape(ex.ToString()));
                    }
                }
            }, settingsBinder);

            return command;
        }

        private class S3Settings
        {
            public string Endpoint { get; set; } = "";
            public string AccessKey { get; set; } = "";
            public string SecretKey { get; set; } = "";
            public string Bucket { get; set; } = "";
            public string KeyPrefix { get; set; } = "";
        }
    }
}
