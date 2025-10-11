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
            if (!string.IsNullOrEmpty(settings.AddMarkdownFile))
            {
                AddMarkdownFile(settings.AddMarkdownFile);
            }
            else if (!string.IsNullOrEmpty(settings.RemoveMarkdownFile))
            {
                RemoveMarkdownFile(settings.RemoveMarkdownFile);
            }
            else if (!string.IsNullOrEmpty(settings.Browser))
            {
                SetBrowser(settings.Browser);
            }
            else if (!string.IsNullOrEmpty(settings.HtmlBookmarkFile))
            {
                SetHtmlBookmarkFile(settings.HtmlBookmarkFile);
            }
            else if (!string.IsNullOrEmpty(settings.BrowserBookmarkPath))
            {
                SetBrowserBookmarkPath(settings.BrowserBookmarkPath);
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
            AnsiConsole.MarkupLine($" • Browser path: '{_tinyCitySettings.BrowserPath}'.");

            string htmlFilePath = !string.IsNullOrEmpty(_tinyCitySettings.HtmlBookmarksFile) ? $"'{_tinyCitySettings.HtmlBookmarksFile}'" : "(none)";
            AnsiConsole.MarkupLine($" • HTML bookmark path: {htmlFilePath}.");

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
        }

        private void AddMarkdownFile(string markdownFile)
        {
            if (!_tinyCitySettings.MarkdownFiles.Contains(markdownFile))
            {
                _tinyCitySettings.MarkdownFiles.Add(markdownFile);
                TinyCitySettings.Save(_tinyCitySettings);
                AnsiConsole.MarkupLine($"[bold green]Added markdown file: {markdownFile}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold red]Markdown file '{markdownFile}' already exists.[/]");
            }
        }

        private void RemoveMarkdownFile(string markdownFile)
        {
            if (_tinyCitySettings.MarkdownFiles.Contains(markdownFile))
            {
                _tinyCitySettings.MarkdownFiles.Remove(markdownFile);
                TinyCitySettings.Save(_tinyCitySettings);
                AnsiConsole.MarkupLine($"[bold green]Removed markdown file: {markdownFile}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold yellow]Markdown file '{markdownFile}' wasn't found in the configuration.[/]");
            }
        }

        private void SetBrowser(string browser)
        {
            string browserLower = browser.ToLowerInvariant();
            string browserPath;
            string bookmarkFullPath;
            
            switch (browserLower)
            {
                case "chrome":
                    browserPath = BrowserKnownPaths.ChromePath;
                    bookmarkFullPath = Path.Combine(browserPath, "Default", "Bookmarks");
                    break;
                case "opera":
                    bookmarkFullPath = BrowserKnownPaths.OperaPath;
                    browserPath = Path.GetDirectoryName(bookmarkFullPath) ?? bookmarkFullPath;
                    break;
                case "brave":
                    bookmarkFullPath = BrowserKnownPaths.BravePath;
                    browserPath = Path.GetDirectoryName(bookmarkFullPath) ?? bookmarkFullPath;
                    break;
                case "edge":
                    bookmarkFullPath = BrowserKnownPaths.EdgePath;
                    browserPath = Path.GetDirectoryName(bookmarkFullPath) ?? bookmarkFullPath;
                    break;
                default:
                    AnsiConsole.MarkupLine($"[bold red]Invalid browser type '{browserLower}'. Valid values are: chrome, opera, brave, edge.[/]");
                    return;
            }

            _tinyCitySettings.BrowserPath = browserPath;
            _tinyCitySettings.BrowserBookmarkFullPath = bookmarkFullPath;
            
            AnsiConsole.MarkupLine($"[bold green]Set browser bookmark path to {bookmarkFullPath}[/]");
            TinyCitySettings.Save(_tinyCitySettings);
        }

        private void SetHtmlBookmarkFile(string htmlBookmarkFile)
        {
            _tinyCitySettings.HtmlBookmarksFile = htmlBookmarkFile;
            TinyCitySettings.Save(_tinyCitySettings);
            AnsiConsole.MarkupLine($"[bold green]Added HTML bookmark file: {htmlBookmarkFile}[/]");
        }

        private void SetBrowserBookmarkPath(string bookmarkPath)
        {
            _tinyCitySettings.BrowserBookmarkFullPath = bookmarkPath;
            TinyCitySettings.Save(_tinyCitySettings);
            AnsiConsole.MarkupLine($"[bold green]Saved browser bookmark path: {bookmarkPath}[/]");
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
