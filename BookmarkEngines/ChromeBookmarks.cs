using Spectre.Console;
using System.Text.Json;
using TinyCity.Model;

namespace TinyCity.BookmarkEngines
{
    public class ChromeBookmarks
    {
        public List<BookmarkNode> FlattenedBookmarks { get; set; } = new List<BookmarkNode>();
        private string _log = "";

        public ChromeBookmarks(TinyCitySettings settings)
        {
            FlattenedBookmarks = new List<BookmarkNode>();

            if (settings.BrowserBookmarkPaths.Count == 0)
            {
                _log = $" {Emoji.Known.Warning} Browser bookmarks: No browser bookmark sources configured.";
                return;
            }

            int totalBookmarksLoaded = 0;
            foreach (var bookmarkPath in settings.BrowserBookmarkPaths)
            {
                if (!File.Exists(bookmarkPath))
                {
                    _log += $" {Emoji.Known.Warning} Browser bookmarks: File not found '{bookmarkPath}'.\n";
                    continue;
                }

                try
                {
                    string json = File.ReadAllText(bookmarkPath);
                    var bookmarks = JsonSerializer.Deserialize(json, TinyCityJsonContext.Default.BookmarksFile);

                    if (bookmarks != null)
                    {
                        var bookmarkBarNodes = FlattenNodes(bookmarks.Roots.BookmarkBar);
                        var otherNodes = FlattenNodes(bookmarks.Roots.Other);
                        var syncedNodes = FlattenNodes(bookmarks.Roots.Synced);

                        var pathBookmarks = new List<BookmarkNode>();
                        pathBookmarks = [.. bookmarkBarNodes, .. otherNodes, .. syncedNodes];

                        FlattenedBookmarks.AddRange(pathBookmarks);
                        totalBookmarksLoaded += pathBookmarks.Count;
                        _log += $" {Emoji.Known.CheckMarkButton} Browser bookmarks: Loaded {pathBookmarks.Count} bookmarks from '{bookmarkPath}'.\n";
                    }
                }
                catch (Exception ex)
                {
                    _log += $" {Emoji.Known.Warning} Browser bookmarks: Error loading '{bookmarkPath}': {ex.Message}\n";
                }
            }

            if (totalBookmarksLoaded == 0 && settings.BrowserBookmarkPaths.Count > 0)
            {
                _log += $" {Emoji.Known.Warning} Browser bookmarks: No bookmarks loaded from {settings.BrowserBookmarkPaths.Count} configured source(s).";
            }
        }

        public string GetLog()
        {
            return _log.TrimEnd('\n');
        }

        static List<BookmarkNode> FlattenNodes(BookmarkNode bookmarkNode)
        {
            if (bookmarkNode.Children == null)
                return new List<BookmarkNode>();

            var bookmarksList = new List<BookmarkNode>();
            foreach (BookmarkNode bookmark in bookmarkNode.Children)
            {
                Recurse(bookmarksList, bookmark);
            }

            return bookmarksList;
        }

        static void Recurse(List<BookmarkNode> list, BookmarkNode bookmark)
        {
            if (bookmark.Children?.Count > 0)
            {
                foreach (var child in bookmark.Children)
                {
                    if (child.Type == "url")
                    {
                        list.Add(child);
                    }

                    if (child.Children?.Count > 0)
                    {
                        Recurse(list, child);
                    }
                }
            }
            else
            {
                list.Add(bookmark);
            }
        }
    }
}
