using System.Text.Json.Serialization;

namespace MinecraftServerlist.Votifier;

internal sealed record VotifierPayload
{
    [JsonInclude, JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonInclude, JsonPropertyName("serviceName")]
    public string? ServiceName { get; set; }

    [JsonInclude, JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    [JsonInclude, JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonInclude, JsonPropertyName("challenge")]
    public string? Challenge { get; set; }
}