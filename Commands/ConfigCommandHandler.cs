using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.CommandLine;
using TinyCity.BookmarkEngines;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{

    public class ConfigCommandHandler : BaseCommandHandler<ConfigCommandSettings>
    {
        private readonly TinyCitySettings _tinyCitySettings;
        private readonly BookmarkAggregator _bookmarkAggregator;

        public ConfigCommandHandler(TinyCitySettings tinyCitySettings, BookmarkAggregator bookmarkAggregator)
        {
            _tinyCitySettings = tinyCitySettings;
            _bookmarkAggregator = bookmarkAggregator;
        }

        public override Task<int> ExecuteAsync(ConfigCommandSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.AddSource))
            {
                AddSource(settings.AddSource);
            }
            else if (!string.IsNullOrEmpty(settings.RemoveSource))
            {
                RemoveSource(settings.RemoveSource);
            }
            else
            {
                ShowConfiguration();
            }

            return Task.FromResult(0);
        }

        private void ShowConfiguration()
        {
            AnsiConsole.MarkupLine($"[turquoise2]Bookmark sources ({_bookmarkAggregator.AllBookmarks.Count} unique bookmarks in total):[/]");
            _bookmarkAggregator.WriteLoadedLog();

            AnsiConsole.MarkupLine($"[turquoise2]Configuration ('{TinyCitySettings.GetConfigFilePath()}'):[/]");
            AnsiConsole.MarkupLine($" • Home Directory: '{_tinyCitySettings.ApplicationConfigDirectory}'.");

            if (_tinyCitySettings.BrowserBookmarkPaths.Count > 0)
            {
                AnsiConsole.MarkupLine($" • Browser Bookmarks:");
                foreach (var file in _tinyCitySettings.BrowserBookmarkPaths)
                {
                    AnsiConsole.MarkupLine($"   • '{file}'");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($" • Browser Bookmarks: (none)");
            }

            if (_tinyCitySettings.MarkdownFiles.Count > 0)
            {
                AnsiConsole.MarkupLine($" • Markdown Files:");
                foreach (var file in _tinyCitySettings.MarkdownFiles)
                {
                    AnsiConsole.MarkupLine($"   • '{file}'");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($" • Markdown Files: (none)");
            }

            if (_tinyCitySettings.HtmlBookmarksFiles.Count > 0)
            {
                AnsiConsole.MarkupLine($" • HTML Bookmark Files:");
                foreach (var file in _tinyCitySettings.HtmlBookmarksFiles)
                {
                    AnsiConsole.MarkupLine($"   • '{file}'");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($" • HTML Bookmark Files: (none)");
            }
        }

        private void AddSource(string source)
        {
            string sourceLower = source.ToLowerInvariant();
            string resolvedPath;
            SourceType sourceType;

            if (sourceLower == "chrome" || sourceLower == "brave" || sourceLower == "edge" || sourceLower == "opera")
            {
                resolvedPath = ResolveBrowserPath(sourceLower);
                sourceType = SourceType.Browser;
            }
            else
            {
                resolvedPath = source;
                sourceType = InferSourceType(source);
            }

            switch (sourceType)
            {
                case SourceType.Browser:
                    if (!_tinyCitySettings.BrowserBookmarkPaths.Contains(resolvedPath))
                    {
                        _tinyCitySettings.BrowserBookmarkPaths.Add(resolvedPath);
                        TinyCitySettings.Save(_tinyCitySettings);
                        AnsiConsole.MarkupLine($"[bold green]Added browser bookmark source: {resolvedPath}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold red]Browser bookmark source '{resolvedPath}' already exists.[/]");
                    }
                    break;

                case SourceType.Markdown:
                    if (!_tinyCitySettings.MarkdownFiles.Contains(resolvedPath))
                    {
                        _tinyCitySettings.MarkdownFiles.Add(resolvedPath);
                        TinyCitySettings.Save(_tinyCitySettings);
                        AnsiConsole.MarkupLine($"[bold green]Added markdown file: {resolvedPath}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold red]Markdown file '{resolvedPath}' already exists.[/]");
                    }
                    break;

                case SourceType.Html:
                    if (!_tinyCitySettings.HtmlBookmarksFiles.Contains(resolvedPath))
                    {
                        _tinyCitySettings.HtmlBookmarksFiles.Add(resolvedPath);
                        TinyCitySettings.Save(_tinyCitySettings);
                        AnsiConsole.MarkupLine($"[bold green]Added HTML bookmark file: {resolvedPath}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold red]HTML bookmark file '{resolvedPath}' already exists.[/]");
                    }
                    break;

                case SourceType.Unknown:
                    AnsiConsole.MarkupLine($"[bold red]Unable to determine source type for '{source}'. Supported types: browser names (chrome, brave, edge, opera), .md files, .html files, or browser bookmark files.[/]");
                    break;
            }
        }

        private void RemoveSource(string source)
        {
            string sourceLower = source.ToLowerInvariant();
            string resolvedPath;
            bool removed = false;

            if (sourceLower == "chrome" || sourceLower == "brave" || sourceLower == "edge" || sourceLower == "opera")
            {
                resolvedPath = ResolveBrowserPath(sourceLower);
                if (_tinyCitySettings.BrowserBookmarkPaths.Contains(resolvedPath))
                {
                    _tinyCitySettings.BrowserBookmarkPaths.Remove(resolvedPath);
                    removed = true;
                }
            }
            else
            {
                resolvedPath = source;

                if (_tinyCitySettings.BrowserBookmarkPaths.Contains(resolvedPath))
                {
                    _tinyCitySettings.BrowserBookmarkPaths.Remove(resolvedPath);
                    removed = true;
                }
                else if (_tinyCitySettings.MarkdownFiles.Contains(resolvedPath))
                {
                    _tinyCitySettings.MarkdownFiles.Remove(resolvedPath);
                    removed = true;
                }
                else if (_tinyCitySettings.HtmlBookmarksFiles.Contains(resolvedPath))
                {
                    _tinyCitySettings.HtmlBookmarksFiles.Remove(resolvedPath);
                    removed = true;
                }
            }

            if (removed)
            {
                TinyCitySettings.Save(_tinyCitySettings);
                AnsiConsole.MarkupLine($"[bold green]Removed source: {resolvedPath}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold yellow]Source '{source}' not found in configuration.[/]");
            }
        }

        private string ResolveBrowserPath(string browser)
        {
            return browser switch
            {
                "chrome" => BrowserKnownPaths.ChromeBookmarksPath,
                "opera" => BrowserKnownPaths.OperaBookmarksPath,
                "brave" => BrowserKnownPaths.BraveBookmarksPath,
                "edge" => BrowserKnownPaths.EdgeBookmarksPath,
                _ => throw new ArgumentException($"Unknown browser type: {browser}")
            };
        }

        private SourceType InferSourceType(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            string filename = Path.GetFileName(path);

            if (extension == ".md")
            {
                return SourceType.Markdown;
            }
            else if (extension == ".html" || extension == ".htm")
            {
                return SourceType.Html;
            }
            else if (filename.Equals("Bookmarks", StringComparison.OrdinalIgnoreCase) ||
                     path.Contains("Chrome", StringComparison.OrdinalIgnoreCase) ||
                     path.Contains("Brave", StringComparison.OrdinalIgnoreCase) ||
                     path.Contains("Edge", StringComparison.OrdinalIgnoreCase) ||
                     path.Contains("Opera", StringComparison.OrdinalIgnoreCase))
            {
                return SourceType.Browser;
            }

            return SourceType.Unknown;
        }

        private enum SourceType
        {
            Browser,
            Markdown,
            Html,
            Unknown
        }

        public override Command CreateCommand(ExtraArgumentHandler extraArgumentHandler)
        {
            var command = new Command("config", "Configure bookmark sources.");

            var settingsBinder = new ConfigCommandSettings();
            settingsBinder.AddOptionsToCommand(command);

            command.SetHandler(async (ConfigCommandSettings settings) =>
            {
                try
                {
                    extraArgumentHandler.SetShowExtraInfo(settings.Extra);
                    await ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
                    AnsiConsole.MarkupLine(Markup.Escape(ex.ToString()));
                }
            }, settingsBinder);

            return command;
        }
    }
}
