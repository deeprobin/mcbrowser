using MinecraftServerlist.Data.Entities.Identity;

namespace MinecraftServerlist.Data.Entities.Servers;

public sealed record ServerVoting
{
    public int Id { get; set; }

    public int ServerId { get; set; }

    public Server Server { get; set; } = default!;

    public int UserId { get; set; }

    public User User { get; set; } = default!;

    public string MinecraftUsername { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}