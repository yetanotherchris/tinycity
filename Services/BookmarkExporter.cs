using System.Linq;
using System.Text;
using System.Text.Json;
using TinyCity.Model;

namespace TinyCity.Services
{
    public static class BookmarkExporter
    {
        public static string ExportToChromiumJson(List<BookmarkNode> bookmarks)
        {
            var bookmarksFile = new BookmarksFile
            {
                Checksum = "",
                Version = 1,
                Roots = new Roots
                {
                    BookmarkBar = new BookmarkNode
                    {
                        Id = "1",
                        Name = "Bookmarks bar",
                        Type = "folder",
                        Children = bookmarks
                    },
                    Other = new BookmarkNode
                    {
                        Id = "2",
                        Name = "Other bookmarks",
                        Type = "folder",
                        Children = new List<BookmarkNode>()
                    },
                    Synced = new BookmarkNode
                    {
                        Id = "3",
                        Name = "Mobile bookmarks",
                        Type = "folder",
                        Children = new List<BookmarkNode>()
                    }
                }
            };

            return JsonSerializer.Serialize(bookmarksFile, TinyCityJsonContext.Default.BookmarksFile);
        }

        public static string ExportToMarkdown(List<BookmarkNode> bookmarks)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Bookmarks");
            sb.AppendLine();

            foreach (var bookmark in bookmarks)
            {
                if (!string.IsNullOrEmpty(bookmark.Url))
                {
                    sb.AppendLine($"- [{bookmark.Name}]({bookmark.Url})");
                }
            }

            return sb.ToString();
        }

        public static string ExportToMarkdownFoldered(List<BookmarkNode> bookmarks, string exportFormat)
        {
            var sb = new StringBuilder();

            var browserBookmarks = bookmarks.Where(b => b.SourceFile == null).ToList();
            var markdownGroups = bookmarks
                .Where(b => b.SourceFile != null)
                .GroupBy(b => b.SourceFile)
                .OrderBy(g => g.Key)
                .ToList();

            var flatBrowserBookmarks = new List<BookmarkNode>();
            var forests = new Dictionary<string, FolderNode>();

            foreach (var bookmark in browserBookmarks)
            {
                if (string.IsNullOrEmpty(bookmark.Url)) continue;

                if (bookmark.FolderPath == null || bookmark.FolderPath.Count <= 1)
                {
                    flatBrowserBookmarks.Add(bookmark);
                    continue;
                }

                string rootName = bookmark.FolderPath[0];
                if (!forests.TryGetValue(rootName, out var root))
                {
                    root = new FolderNode { Name = rootName, IsRoot = true };
                    forests[rootName] = root;
                }

                var node = root;
                foreach (var folderName in bookmark.FolderPath.Skip(1))
                    node = node.GetOrAddChild(folderName);
                node.Bookmarks.Add(bookmark);
            }

            foreach (var bookmark in flatBrowserBookmarks.OrderBy(x => x.Name))
                sb.AppendLine(FormatLink(bookmark, exportFormat));
            if (flatBrowserBookmarks.Count > 0)
                sb.AppendLine();

            foreach (var forest in forests.Values.OrderBy(x => x.Name))
                WalkFolders(forest, sb, 2, new List<string>(), exportFormat);

            foreach (var group in markdownGroups)
            {
                sb.AppendLine($"## {group.Key ?? "Markdown bookmarks"}");
                sb.AppendLine();
                foreach (var bookmark in group.OrderBy(b => b.Name))
                    sb.AppendLine(FormatLink(bookmark, exportFormat));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void WalkFolders(FolderNode node, StringBuilder sb, int headingLevel, List<string> chain, string exportFormat)
        {
            if (node.IsRoot)
            {
                foreach (var child in node.Children.Values.OrderBy(x => x.Name))
                    WalkFolders(child, sb, headingLevel, new List<string>(), exportFormat);
                return;
            }

            bool hasBookmarks = node.Bookmarks.Count > 0;
            bool isMultiBranch = node.Children.Count >= 2;

            if (hasBookmarks || isMultiBranch)
            {
                string title = chain.Count > 0
                    ? string.Join(" -> ", [.. chain, node.Name])
                    : node.Name;
                sb.AppendLine($"{new string('#', Math.Min(headingLevel, 4))} {title}");
                sb.AppendLine();

                foreach (var bookmark in node.Bookmarks.OrderBy(x => x.Name))
                    sb.AppendLine(FormatLink(bookmark, exportFormat));
                if (hasBookmarks)
                    sb.AppendLine();

                foreach (var child in node.Children.Values.OrderBy(x => x.Name))
                    WalkFolders(child, sb, headingLevel + 1, new List<string>(), exportFormat);
            }
            else if (node.Children.Count > 0)
            {
                // Single-branch empty folder: accumulate in chain for arrow notation
                foreach (var child in node.Children.Values.OrderBy(x => x.Name))
                    WalkFolders(child, sb, headingLevel, [.. chain, node.Name], exportFormat);
            }
        }

        private static string FormatLink(BookmarkNode bookmark, string exportFormat)
        {
            string urlHost;
            if (bookmark.Url != null && bookmark.Url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            {
                urlHost = "file://";
            }
            else
            {
                try
                {
                    urlHost = new Uri(bookmark.Url!).Host;
                }
                catch
                {
                    urlHost = "";
                }
            }

            string result = exportFormat
                .Replace("{name}", bookmark.Name)
                .Replace("{url}", bookmark.Url);

            if (string.IsNullOrEmpty(urlHost))
            {
                result = result
                    .Replace(" ({urlhost})", "")
                    .Replace("{urlhost}", "");
            }
            else
            {
                result = result.Replace("{urlhost}", urlHost);
            }

            return result;
        }

        public static string ExportToHtml(List<BookmarkNode> bookmarks)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE NETSCAPE-Bookmark-file-1>");
            sb.AppendLine("<!-- This is an automatically generated file.");
            sb.AppendLine("     It will be read and overwritten.");
            sb.AppendLine("     DO NOT EDIT! -->");
            sb.AppendLine("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");
            sb.AppendLine("<TITLE>Bookmarks</TITLE>");
            sb.AppendLine("<H1>Bookmarks</H1>");
            sb.AppendLine("<DL><p>");

            foreach (var bookmark in bookmarks)
            {
                if (!string.IsNullOrEmpty(bookmark.Url))
                {
                    sb.AppendLine($"    <DT><A HREF=\"{bookmark.Url}\">{bookmark.Name}</A>");
                }
            }

            sb.AppendLine("</DL><p>");
            return sb.ToString();
        }

        private sealed class FolderNode
        {
            public string Name { get; init; } = "";

            public bool IsRoot { get; init; }

            public List<BookmarkNode> Bookmarks { get; } = new List<BookmarkNode>();

            public Dictionary<string, FolderNode> Children { get; } = new Dictionary<string, FolderNode>();

            public FolderNode GetOrAddChild(string name)
            {
                if (!Children.TryGetValue(name, out var child))
                {
                    child = new FolderNode { Name = name };
                    Children[name] = child;
                }
                return child;
            }
        }
    }
}
