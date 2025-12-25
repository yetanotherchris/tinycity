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
    }
}
