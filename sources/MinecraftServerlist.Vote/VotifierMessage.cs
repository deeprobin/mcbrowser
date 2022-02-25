using System.Text.Json.Serialization;

namespace MinecraftServerlist.Votifier;

internal sealed record VotifierMessage
{
    [JsonPropertyName("signature")]
    [JsonInclude]
    public string? Signature { get; set; }

    [JsonPropertyName("payload")]
    [JsonInclude]
    public string? Payload { get; set; }
}