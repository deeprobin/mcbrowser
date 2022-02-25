namespace MinecraftServerlist.InternalApi.Common.Bodies;

public record LoginBody
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}