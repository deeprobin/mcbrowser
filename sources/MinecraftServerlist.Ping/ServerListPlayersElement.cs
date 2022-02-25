using System.Text.Json.Serialization;

namespace MinecraftServerlist.Ping;

public sealed record ServerListPlayersElement
{
    [JsonPropertyName("max")]
    [JsonInclude]
    public int? MaxPlayers { get; set; }

    [JsonPropertyName("online")]
    [JsonInclude]
    public int? OnlinePlayers { get; set; }

    [JsonPropertyName("sample")]
    [JsonInclude]
    public IEnumerable<ServerListPlayerSample>? Samples { get; set; }
}