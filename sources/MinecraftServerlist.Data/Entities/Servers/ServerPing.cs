namespace MinecraftServerlist.Data.Entities.Servers;

public sealed record ServerPing
{
    public long Id { get; set; }

    public int ServerId { get; set; }

    public bool Online { get; set; }

    public Server Server { get; set; } = default!;

    public string? MessageOfTheDay { get; set; }

    public int? OnlinePlayers { get; set; }

    public int? MaxPlayers { get; set; }

    public string? VersionName { get; set; }

    public int? VersionProtocol { get; set; }

    public DateTime CreatedAt { get; set; }
}