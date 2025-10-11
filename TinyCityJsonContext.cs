using System.Text.Json.Serialization;
using TinyCity.Model;

namespace TinyCity
{
    [JsonSerializable(typeof(TinyCitySettings))]
    [JsonSerializable(typeof(BookmarksFile))]
    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
    internal partial class TinyCityJsonContext : JsonSerializerContext
    {
    }
}
