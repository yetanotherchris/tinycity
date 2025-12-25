using TinyCity.Model;
using AngleSharp;
using AngleSharp.Dom;
using Spectre.Console;

namespace TinyCity.BookmarkEngines
{
    /// <summary>
    /// HTML bookmarks exports, using the Netscape format. The HTML Format is:
    /// <![CDATA[
    /// <DT><A HREF=..
    /// ]]>
    /// However, this parser doesn't care, it just grabs all the <a> tags and their href attributes.
    /// </summary>
    public class HtmlBookmarks
    {
        public List<BookmarkNode> Bookmarks { get; set; } = new List<BookmarkNode>();
        private string _log = "";

        public HtmlBookmarks(TinyCitySettings settings)
        {
            if (settings.HtmlBookmarksFiles.Count == 0)
            {
                _log = $" {Emoji.Known.Prohibited} HTML bookmarks: No HTML bookmark files configured.";
                return;
            }

            int totalBookmarksLoaded = 0;
            foreach (var htmlFilePath in settings.HtmlBookmarksFiles)
            {
                if (!File.Exists(htmlFilePath))
                {
                    _log += $" {Emoji.Known.Warning} HTML bookmarks: File not found '{htmlFilePath}'.\n";
                    continue;
                }

                try
                {
                    string html = File.ReadAllText(htmlFilePath);
                    var bookmarks = ParseHtmlFile(html).GetAwaiter().GetResult();
                    Bookmarks.AddRange(bookmarks);
                    totalBookmarksLoaded += bookmarks.Count;
                    _log += $" {Emoji.Known.CheckMarkButton} HTML bookmarks: Loaded {bookmarks.Count} bookmarks from '{htmlFilePath}'.\n";
                }
                catch (Exception ex)
                {
                    _log += $" {Emoji.Known.Warning} HTML bookmarks: Error loading '{htmlFilePath}': {ex.Message}\n";
                }
            }

            if (totalBookmarksLoaded == 0 && settings.HtmlBookmarksFiles.Count > 0)
            {
                _log += $" {Emoji.Known.Warning} HTML bookmarks: No bookmarks loaded from {settings.HtmlBookmarksFiles.Count} configured source(s).";
            }
        }

        public string GetLog()
        {
            return _log.TrimEnd('\n');
        }

        private async Task<List<BookmarkNode>> ParseHtmlFile(string html)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(m => m.Content(html));

            var bookmarks = new List<BookmarkNode>();
            foreach (var anchor in document.QuerySelectorAll("a"))
            {
                string text = anchor.Text();
                string? href = anchor.Attributes["href"]?.Value;

                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(href))
                {
                    var bookmark = new BookmarkNode
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = text,
                        Url = href,
                        Type = "url"
                    };

                    bookmarks.Add(bookmark);
                }

            }

            return bookmarks;
        }
    }
}
