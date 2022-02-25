namespace MinecraftServerlist.Data.Entities.Financial.Stripe;

public sealed record StripePayment : Payment
{
    public override int Id { get; set; }

    public string StripeId { get; set; } = default!;

    public string StripePaymentIntentId { get; set; } = default!;

    public string StripeInvoiceId { get; set; } = default!;

    public string Locale { get; set; } = default!;

    public string CustomerId { get; set; } = default!;

    public string? CustomerEmail { get; set; }

    public long AmountInCents { get; set; }

    public string Currency { get; set; } = default!;

    public DateTime? CanceledAt { get; set; }

    public StripePaymentStatus Status { get; set; }

    public StripePaymentCancellationReason? CancellationReason { get; set; }
}