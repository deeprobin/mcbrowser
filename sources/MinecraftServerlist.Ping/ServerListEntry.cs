using System.Text.Json.Serialization;

namespace MinecraftServerlist.Ping;

public sealed record ServerListEntry
{
    [JsonPropertyName("version")]
    [JsonInclude]
    public ServerListVersion? Version { get; set; }

    [JsonPropertyName("players")]
    [JsonInclude]
    public ServerListPlayersElement? PlayersElement { get; set; }

    [JsonPropertyName("description")]
    [JsonInclude]
    public object? Description { get; set; }

    [JsonPropertyName("favicon")]
    [JsonInclude]
    public string? FaviconBase64 { get; set; }
}