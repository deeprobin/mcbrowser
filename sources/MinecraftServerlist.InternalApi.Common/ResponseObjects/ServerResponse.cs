using System.Text.Json.Serialization;

namespace MinecraftServerlist.InternalApi.Common.ResponseObjects;

public record ServerResponse
{
    [JsonInclude, JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonInclude, JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonInclude, JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    [JsonInclude, JsonPropertyName("online_players")]
    public int? OnlinePlayers { get; set; }

    [JsonInclude, JsonPropertyName("max_players")]
    public int? MaxPlayers { get; set; }

    [JsonInclude, JsonPropertyName("vote_count")]
    public int? VoteCount { get; set; }
}