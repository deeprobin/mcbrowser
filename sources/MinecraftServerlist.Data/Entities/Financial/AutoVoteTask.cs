using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.Entities.Financial;

public sealed record AutoVoteTask
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int TargetServerId { get; set; } = default!;

    public Server TargetServer { get; set; } = default!;

    // UUID of minecraft user
    public byte[] TargetUuid { get; set; } = default!;

    public int? PaymentId { get; set; }

    public Payment? Payment { get; set; }
}