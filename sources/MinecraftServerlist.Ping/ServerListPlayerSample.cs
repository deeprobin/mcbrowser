using System.Text.Json.Serialization;

namespace MinecraftServerlist.Ping;

public sealed record ServerListPlayerSample
{
    [JsonPropertyName("id")]
    [JsonInclude]
    public string? UniqueId { get; set; }

    [JsonPropertyName("name")]
    [JsonInclude]
    public string? Name { get; set; }
}