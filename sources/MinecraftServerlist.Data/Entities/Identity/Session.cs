namespace MinecraftServerlist.Data.Entities.Identity;

public record Session
{
    public long Id { get; set; }

    public int ParentUserId { get; set; }

    public User User { get; set; } = default!;

    // Ipv4 = 4 bytes
    // Ipv6 = 16 bytes
    public byte[] Ip { get; set; } = default!;

    public byte[] TokenBytes { get; set; } = default!;

    public string UserAgent { get; set; } = default!;

    public bool Revoked { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ValidUntil { get; set; }
}