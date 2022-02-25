namespace MinecraftServerlist.Data.Entities.Financial.Stripe;

public enum StripePaymentStatus
{
    Canceled,
    Processing,
    RequiresAction,
    RequiresCapture,
    RequiresConfirmation,
    RequiresPaymentMethod,
    Succeeded
}