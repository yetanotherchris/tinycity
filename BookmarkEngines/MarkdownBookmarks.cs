using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Spectre.Console;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class MarkdownBookmarks
    {
        public List<BookmarkNode> Bookmarks { get; set; } = new List<BookmarkNode>();
        private List<string> logItems = new List<string>();

        public MarkdownBookmarks(TinyCitySettings settings)
        {
            if (settings.MarkdownFiles.Count == 0)
            {
                logItems.Add($" {Emoji.Known.Prohibited} Markdown bookmarks: no files specified in the settings.");
                return;
            }

            foreach (var file in settings.MarkdownFiles)
            {
                if (File.Exists(file))
                {
                    string markdown = File.ReadAllText(file);
                    var bookmarks = ParseMarkdownFile(markdown);
                    string fileName = Path.GetFileName(file);
                    foreach (var bm in bookmarks) bm.SourceFile = fileName;
                    Bookmarks.AddRange(bookmarks);

                    logItems.Add($" {Emoji.Known.CheckMarkButton} Markdown bookmarks: Loaded {bookmarks.Count} bookmarks from '{file}'.");
                }
                else
                {
                    logItems.Add($" {Emoji.Known.Warning} Markdown bookmarks: couldn't find '{file}' so skipping.");
                }
            }
        }

        public List<string> GetLog()
        {
            return logItems;
        }

        private List<BookmarkNode> ParseMarkdownFile(string markdown)
        {
            var document = Markdown.Parse(markdown);

            var bookmarks = new List<BookmarkNode>();
            foreach (var inline in document.Descendants<LinkInline>())
            {
                if (inline is LinkInline linkInline)
                {
                    string linkText = linkInline.FirstChild?.ToString() ?? string.Empty;
                    string url = linkInline.Url;

                    if (!string.IsNullOrEmpty(linkText) && !string.IsNullOrEmpty(url))
                    {
                        var bookmark = new BookmarkNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = linkText,
                            Url = url,
                            Type = "url"
                        };

                        bookmarks.Add(bookmark);
                    }
                }
            }

            return bookmarks;
        }
    }
}
