using System.Text.Json.Serialization;

namespace MinecraftServerlist.Votifier;

internal sealed record VotifierResult
{
    [JsonPropertyName("status")]
    [JsonInclude]
    public string? Status { get; set; }

    [JsonPropertyName("error")]
    [JsonInclude]
    public string? Error { get; set; }
}