using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Text;
using TinyCity.BookmarkEngines;
using TinyCity.Model;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{
    public class SearchCommandHandler : BaseCommandHandler<SearchCommandSettings>
    {
        private readonly List<BookmarkNode> _combinedBookmarks;

        public SearchCommandHandler(BookmarkAggregator bookmarkAggregator)
        {
            _combinedBookmarks = bookmarkAggregator.AllBookmarks;
        }

        public override Task<int> ExecuteAsync(SearchCommandSettings settings)
        {
            var exportStringBuilder = new StringBuilder();
            var filteredBookmarks = Search(settings.Query, settings.SearchUrls);
            int count = filteredBookmarks.Count;
            if (count == 0)
            {
                AnsiConsole.MarkupLine($"[bold yellow]No bookmarks found for '{settings.Query}'.[/]");
                return Task.FromResult(0);
            }

            AnsiConsole.MarkupLine($"[bold turquoise2]{count} bookmark(s) found for '{settings.Query}'.[/]");
            foreach (var bookmark in filteredBookmarks)
            {
                if (!string.IsNullOrEmpty(bookmark.Url))
                {
                    string bookmarkUrl = Markup.Escape(bookmark.Url);
                    string bookmarkName = Markup.Escape(bookmark.Name);

                    string link = $"[link={bookmarkUrl}]{bookmarkName}[/]";
                    string urlHost = new Uri(bookmark.Url).Host;
                    AnsiConsole.MarkupLine($" • [bold chartreuse1]{link}[/] ({urlHost})");

                    string exportLine = settings.ExportFormat
                                                .Replace("{name}", Markup.Escape(bookmarkName))
                                                .Replace("{url}", bookmarkUrl)
                                                .Replace("{urlhost}", urlHost);

                    exportStringBuilder.AppendLine(exportLine);
                }
            }

            if (settings.Launch)
            {
                var firstItem = filteredBookmarks.FirstOrDefault();
                if (firstItem != null)
                {
                    string escapedName = Markup.Escape(firstItem.Name);
                    AnsiConsole.MarkupLine($"[bold green]Launching '{escapedName}'[/]...");

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = firstItem.Url,
                        UseShellExecute = true
                    };

                    Process.Start(startInfo);
                }
            }
            else if (settings.Export)
            {
                File.WriteAllText("exported-bookmarks.md", exportStringBuilder.ToString());
                AnsiConsole.MarkupLine($"[bold green]Exported search results to 'exported-bookmarks.md'[/].");
            }

            return Task.FromResult(0);
        }

        private List<BookmarkNode> Search(string searchTerm, bool searchUrls)
        {
            searchTerm = searchTerm.ToLower();

            if (searchUrls)
            {
                return _combinedBookmarks
                      .Where(b => b.Name.ToLower().Contains(searchTerm) || (b.Url != null && b.Url.ToLower().Contains(searchTerm)))
                      .OrderBy(x => x.Name)
                      .ToList();
            }
            else
            {
                return _combinedBookmarks
                       .Where(b => b.Name.ToLower().Contains(searchTerm))
                       .OrderBy(x => x.Name)
                       .ToList();
            }
        }

        public override Command CreateCommand(ExtraArgumentHandler extraArgumentHandler)
        {
            var command = new Command("search", "Search the bookmarks.");
            command.AddAlias("q");

            var settings = new SearchCommandSettings();
            settings.AddOptionsToCommand(command);

            command.SetHandler(async (SearchCommandSettings settings) =>
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
            }, settings);

            return command;
        }
    }
}
