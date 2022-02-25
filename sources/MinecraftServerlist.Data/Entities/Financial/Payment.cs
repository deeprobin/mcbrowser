namespace MinecraftServerlist.Data.Entities.Financial;

public record Payment
{
    public virtual int Id { get; set; }

    public IEnumerable<AutoVoteTask>? AutoVoteTasks { get; set; }

    public int? PaymentOperationId { get; set; }

    public PaymentOperation? PaymentOperation { get; set; }
}