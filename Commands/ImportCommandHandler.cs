using Spectre.Console;
using System.CommandLine;
using TinyCity.Commands.Settings;
using TinyCity.Services;

namespace TinyCity.Commands
{
    public class ImportCommandHandler : BaseCommandHandler<ImportCommandSettings>
    {
        private readonly TinyCitySettings _settings;
        private readonly FileBackupService _backup;
        private readonly BookmarkImporter _importer;

        public ImportCommandHandler(
            TinyCitySettings settings,
            FileBackupService backup,
            BookmarkImporter importer)
        {
            _settings = settings;
            _backup = backup;
            _importer = importer;
        }

        public override async Task<int> ExecuteAsync(ImportCommandSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Target))
            {
                AnsiConsole.MarkupLine("[red]Option --target is required. Valid values: chrome, brave, edge, opera, markdown, html, all[/]");
                return 1;
            }

            if (string.IsNullOrEmpty(settings.Directory))
            {
                AnsiConsole.MarkupLine("[red]Option --directory is required[/]");
                return 1;
            }

            if (!Directory.Exists(settings.Directory))
            {
                AnsiConsole.MarkupLine($"[red]Local directory '{settings.Directory}' not found[/]");
                return 1;
            }

            string target = settings.Target.ToLowerInvariant();

            if (target == "all")
            {
                await ImportAllFromLocal(settings.Directory);
            }
            else
            {
                await ImportSingleFromLocal(target, settings.Directory);
            }

            return 0;
        }

        private async Task ImportAllFromLocal(string directory)
        {
            var importedFiles = new List<string>();
            var skippedFiles = new List<string>();

            var filesToTry = new[]
            {
                ("chrome.json", "chrome"),
                ("brave.json", "brave"),
                ("edge.json", "edge"),
                ("opera.json", "opera"),
                ("bookmarks.md", "markdown"),
                ("bookmarks.html", "html")
            };

            foreach (var (filename, targetType) in filesToTry)
            {
                string filePath = Path.Combine(directory, filename);

                if (!File.Exists(filePath))
                {
                    skippedFiles.Add($"{filename} (not found)");
                    continue;
                }

                try
                {
                    string? targetPath = GetTargetPath(targetType);
                    if (string.IsNullOrEmpty(targetPath))
                    {
                        skippedFiles.Add($"{filename} (not configured)");
                        continue;
                    }

                    await ImportFileFromLocal(filePath, targetPath, targetType);
                    importedFiles.Add(filename);
                }
                catch (Exception ex)
                {
                    skippedFiles.Add($"{filename} ({ex.Message})");
                }
            }

            if (importedFiles.Count > 0)
            {
                AnsiConsole.MarkupLine($"[bold green]Imported {importedFiles.Count} file(s): {string.Join(", ", importedFiles)}[/]");
            }

            if (skippedFiles.Count > 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Skipped {skippedFiles.Count} file(s): {string.Join(", ", skippedFiles)}[/]");
            }

            if (importedFiles.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No files were imported[/]");
            }
        }

        private async Task ImportSingleFromLocal(string target, string directory)
        {
            string? targetPath = GetTargetPath(target);
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new InvalidOperationException($"Cannot import to {target}: not configured. Run 'tinycity config' to configure.");
            }

            string filename = GetFilenameForTarget(target);
            string filePath = Path.Combine(directory, filename);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            await ImportFileFromLocal(filePath, targetPath, target);

            AnsiConsole.MarkupLine($"[bold green]Imported {filename} to {targetPath}[/]");
            AnsiConsole.MarkupLine($"[dim]Backup created at: {targetPath}.bak[/]");
        }

        private async Task ImportFileFromLocal(string filePath, string targetPath, string targetType)
        {
            string content = await File.ReadAllTextAsync(filePath);

            if (targetType == "chrome" || targetType == "brave" || targetType == "edge" || targetType == "opera")
            {
                await _importer.ImportFromChromiumJsonAsync(content, targetPath, _backup);
            }
            else if (targetType == "markdown")
            {
                await _importer.ImportFromMarkdownAsync(content, targetPath, _backup);
            }
            else if (targetType == "html")
            {
                await _importer.ImportFromHtmlAsync(content, targetPath, _backup);
            }
        }

        private string? GetTargetPath(string target)
        {
            string targetLower = target.ToLowerInvariant();

            if (targetLower == "chrome" || targetLower == "brave" || targetLower == "edge" || targetLower == "opera")
            {
                foreach (var browserPath in _settings.BrowserBookmarkPaths)
                {
                    string browserType = GetBrowserTypeFromPath(browserPath);
                    if (browserType.Equals(targetLower, StringComparison.OrdinalIgnoreCase))
                    {
                        return browserPath;
                    }
                }
                return null;
            }
            else if (targetLower == "markdown")
            {
                return _settings.MarkdownFiles.Count > 0 ? _settings.MarkdownFiles[0] : null;
            }
            else if (targetLower == "html")
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

        private string GetFilenameForTarget(string target)
        {
            return target.ToLowerInvariant() switch
            {
                "chrome" => "chrome.json",
                "brave" => "brave.json",
                "edge" => "edge.json",
                "opera" => "opera.json",
                "markdown" => "bookmarks.md",
                "html" => "bookmarks.html",
                _ => $"{target}.json"
            };
        }

        public override Command CreateCommand(ExtraArgumentHandler extraArgumentHandler)
        {
            var command = new Command("import", "Import bookmarks from local filesystem.");

            var settingsBinder = new ImportCommandSettings();
            settingsBinder.AddOptionsToCommand(command);

            command.SetHandler(async (ImportCommandSettings settings) =>
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
