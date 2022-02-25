namespace MinecraftServerlist.Data.Entities.Financial;

public record PaymentOperation
{
    public long Id { get; set; }

    public PaymentOperationState State { get; set; }

    public Payment? Payment { get; set; }

    public int? PaymentId { get; set; }

    public string? PaymentProviderSessionId { get; set; }

    public DateTime CreatedAt { get; set; }
}