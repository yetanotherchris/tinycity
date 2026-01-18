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
            if (string.IsNullOrEmpty(settings.Source))
            {
                AnsiConsole.MarkupLine("[red]Option --source is required. Valid values: chrome, brave, edge, opera, markdown, html, all[/]");
                return 1;
            }

            if (string.IsNullOrEmpty(settings.Directory))
            {
                AnsiConsole.MarkupLine("[red]Option --directory is required[/]");
                return 1;
            }

            if (!Directory.Exists(settings.Directory))
            {
                Directory.CreateDirectory(settings.Directory);
            }

            string source = settings.Source.ToLowerInvariant();

            if (source == "all")
            {
                await ExportAllToLocal(settings.Directory);
            }
            else
            {
                await ExportSingleToLocal(source, settings.Directory);
            }

            return 0;
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
            var command = new Command("export", "Export bookmarks to local filesystem.");

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
    }
}
