using System.Text.Json.Serialization;

namespace TinyCity.Model
{
    public class BookmarkNode : IEquatable<BookmarkNode>
    {
        [JsonPropertyName("date_added")]
        public string? DateAdded { get; set; }

        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("date_modified")]
        public string? DateModified { get; set; }

        [JsonPropertyName("children")]
        public List<BookmarkNode>? Children { get; set; }

        [JsonIgnore]
        public List<string>? FolderPath { get; set; }

        public bool Equals(BookmarkNode? other)
        {
            if (other is null) return false;
            return Url == other.Url && Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BookmarkNode);
        }

        public override int GetHashCode()
        {
            return Url?.GetHashCode() ?? 0;
        }
    }
}