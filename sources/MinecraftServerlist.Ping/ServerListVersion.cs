using System.Text.Json.Serialization;

namespace MinecraftServerlist.Ping;

public sealed record ServerListVersion
{
    [JsonPropertyName("name")]
    [JsonInclude]
    public string? Name { get; set; }

    [JsonPropertyName("protocol")]
    [JsonInclude]
    public int? Protocol { get; set; }
}