namespace MinecraftServerlist.InternalApi.Common.ResponseObjects;

public record LoginResponse
{
    public string? SessionToken { get; set; }
}