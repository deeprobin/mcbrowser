using System.Text.Json.Serialization;

namespace MinecraftServerlist.InternalApi.Common.ResponseObjects;

public sealed record AdvancedServerResponse : ServerResponse
{
    [JsonInclude, JsonPropertyName("owner_id")]
    public int? OwnerId { get; set; }

    [JsonInclude, JsonPropertyName("server_address")]
    public string? ServerAddress { get; set; }

    [JsonInclude, JsonPropertyName("server_port")]
    public ushort? ServerPort { get; set; }

    [JsonInclude, JsonPropertyName("long_description")]
    public string? LongDescription { get; set; }

    [JsonInclude, JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonInclude, JsonPropertyName("teamspeak_address")]
    public string? TeamspeakAddress { get; set; }

    [JsonInclude, JsonPropertyName("discord_invitation_id")]
    public string? DiscordInvitationId { get; set; }
}