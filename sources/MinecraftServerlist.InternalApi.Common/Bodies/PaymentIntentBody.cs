using System.Text.Json.Serialization;

namespace MinecraftServerlist.InternalApi.Common.Bodies;

public record PaymentIntentBody
{
    [JsonInclude] public string? ClientSecret;
}