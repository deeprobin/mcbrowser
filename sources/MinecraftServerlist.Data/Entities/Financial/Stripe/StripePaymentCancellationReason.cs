namespace MinecraftServerlist.Data.Entities.Financial.Stripe;

public enum StripePaymentCancellationReason
{
    Duplicate,
    Fraudulent,
    RequestedByCustomer,
    Abandoned,
    FailedInvoice,
    VoidInvoice,
    Automatic
}