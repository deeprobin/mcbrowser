using MinecraftServerlist.Data.Entities.Identity;

namespace MinecraftServerlist.Data.Entities.Financial.Stripe;

public sealed record StripeRefund
{
    public int Id { get; set; }

    public int AmountInCents { get; set; }

    public string StripeId { get; set; } = default!;

    public string? Description { get; set; }

    public string ChargeId { get; set; } = default!;

    public string? PaymentIntentId { get; set; }

    // 3-letter-iso-code
    public string CurrencyCode { get; set; } = default!;

    public StripeRefundReason RefundReason { get; set; } = default!;

    public StripeRefundReason RefundStatus { get; set; } = default!;

    public int UserId { get; set; }

    public User User { get; set; } = default!;
}