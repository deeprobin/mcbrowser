namespace MinecraftServerlist.Data.Entities.Financial.Stripe;

// Status of the refund. For credit card refunds, this can be pending, succeeded, or failed. For other types of refunds, it can be pending, succeeded, failed, or canceled. Refer to our refunds documentation for more details.
// https://stripe.com/docs/api/refunds/object
public enum StripeRefundStatus
{
    Pending,
    Succeeded,
    Failed,
    Canceled
}