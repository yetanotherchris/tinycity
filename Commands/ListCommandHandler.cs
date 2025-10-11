using Spectre.Console;
using System.CommandLine;
using System.Text;
using TinyCity.BookmarkEngines;
using TinyCity.Model;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{
    public class ListCommandHandler : BaseCommandHandler<ListCommandSettings>
    {
        private readonly List<BookmarkNode> _combinedBookmarks;

        public ListCommandHandler(BookmarkAggregator bookmarkAggregator)
        {
            _combinedBookmarks = bookmarkAggregator.AllBookmarks;
        }

        public override Task<int> ExecuteAsync(ListCommandSettings settings)
        {
            var exportStringBuilder = new StringBuilder();
            AnsiConsole.MarkupLine($"[bold turquoise2]{_combinedBookmarks.Count} unique bookmarks in total.[/]");

            foreach (var bookmark in _combinedBookmarks.OrderBy(x => x.Name))
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

            if (settings.Export)
            {
                File.WriteAllText("exported-bookmarks.md", exportStringBuilder.ToString());
                AnsiConsole.MarkupLine($"[bold green]Exported to all bookmarks 'exported-bookmarks.md'[/].");
            }

            return Task.FromResult(0);
        }

        public override Command CreateCommand(ExtraArgumentHandler extraArgumentHandler)
        {
            var command = new Command("ls", "List all bookmarks.");
            command.AddAlias("list");

            var settingsBinder = new ListCommandSettings();
            settingsBinder.AddOptionsToCommand(command);

            command.SetHandler(async (ListCommandSettings settings) =>
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
