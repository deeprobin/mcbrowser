using MinecraftServerlist.Data.Entities.Servers;
using Stripe;
using Session = Stripe.Checkout.Session;

namespace MinecraftServerlist.Services.Abstractions;

public interface IPaymentService
{
    public Task MapWebHookAsync(string url, CancellationToken cancellationToken = default);

    public Task ReceiveWebHookAsync(Event stripeEvent, CancellationToken cancellationToken = default);

    public Task<PaymentIntent> CreateAutoVotePaymentIntentAsync(CancellationToken cancellationToken = default);

    public Task<Stripe.BillingPortal.Session> CreatePortalSessionAsync(string returnUrl, string sessionId,
        CancellationToken cancellationToken = default);

    public Task<Session> CreateAutoVotingSessionAsync(Server server, string successUrl, string cancelUrl,
        CancellationToken cancellationToken = default);

    internal Task RefreshPayments(CancellationToken cancellationToken = default);
}