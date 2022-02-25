using MinecraftServerlist.Data.Entities.Financial.Stripe;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.Entities.Identity;

public sealed record User
{
    public int Id { get; set; }

    public string MailAddress { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public byte[] HashedPassword { get; set; } = default!;

    public DateTime LastUpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserRole UserRole { get; set; }

    public UserState UserState { get; set; }

    public IEnumerable<Server> Servers { get; set; } = default!;

    public IEnumerable<Session> Sessions { get; set; } = default!;

    public IEnumerable<ServerVoting> SubmittedVotings { get; set; } = default!;

    public IEnumerable<StripeRefund> StripeRefunds { get; set; } = default!;
}