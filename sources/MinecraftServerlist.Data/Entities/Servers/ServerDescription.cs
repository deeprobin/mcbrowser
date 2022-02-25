namespace MinecraftServerlist.Data.Entities.Servers;

public sealed record ServerDescription
{
    public int Id { get; set; }

    public int ServerId { get; set; }

    public Server Server { get; set; } = default!;

    public string Culture { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string ShortDescription { get; set; } = default!;

    public string LongDescription { get; set; } = default!;

    public string? Website { get; set; }

    public string? DiscordInvitationId { get; set; }

    public string? TeamspeakAddress { get; set; }
}