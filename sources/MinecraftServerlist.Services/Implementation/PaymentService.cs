using MinecraftServerlist.Data.Entities.Financial;
using MinecraftServerlist.Data.Entities.Financial.Stripe;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;
using Stripe;
using Stripe.Checkout;
using System.Collections.Concurrent;
using Session = Stripe.Checkout.Session;
using WebhookEndpointCreateOptions = Stripe.WebhookEndpointCreateOptions;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class PaymentService : IPaymentService
{
    // TODO: Move into Configuration / ENV
    private const string ApiSecretKey =
        "*** censored ***";

    private const string AutoVotingPriceId = "price_1KO3DqKkXsyfF6NDfgjLq0M0";

    private readonly StripeClient _stripeClient;
    private readonly PostgresDbContext _dbContext;

    private readonly IDictionary<long, Session> _temporaryOperationSessionMap;

    public PaymentService(HttpClient httpClient, PostgresDbContext dbContext)
    {
        // This is a payment service
        // We don't want to lose money, so retry 6 times
        const int maxRetries = 6;

        var stripeClient = new StripeClient(ApiSecretKey,
            httpClient: new SystemNetHttpClient(httpClient, maxRetries));

        StripeConfiguration.StripeClient = stripeClient;
        StripeConfiguration.MaxNetworkRetries = maxRetries;

        _stripeClient = stripeClient;
        _dbContext = dbContext;

        // Concurrency is required, because `CreateAutoVotingSessionAsync` gets called by different threads
        _temporaryOperationSessionMap = new ConcurrentDictionary<long, Session>();
    }

    public async Task MapWebHookAsync(string url, CancellationToken cancellationToken = default)
    {
        var options = new WebhookEndpointCreateOptions
        {
            Url = "https://example.com/my/webhook/endpoint",
            EnabledEvents = new List<string>
            {
                "*"
                //"charge.failed",
                //"charge.succeeded",
            },
        };
        var service = new WebhookEndpointService();
        var endpoint = await service.CreateAsync(options, cancellationToken: cancellationToken);
    }

    public async Task ReceiveWebHookAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        // Handle the event
        if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            // Then define and call a method to handle the successful payment intent.
            // handlePaymentIntentSucceeded(paymentIntent);
        }
        else if (stripeEvent.Type == Events.PaymentMethodAttached)
        {
            var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
            // Then define and call a method to handle the successful attachment of a PaymentMethod.
            // handlePaymentMethodAttached(paymentMethod);
        }
        else if (stripeEvent.Type == Events.InvoiceFinalized)
        {
            var invoice = stripeEvent.Data.Object as Invoice;
            var paymentOperationDbSet = _dbContext.PaymentOperationDbSet;
        }
        // ... handle other event types
        else
        {
            // Unexpected event type
            Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
        }
    }

    public Task<PaymentIntent> CreateAutoVotePaymentIntentAsync(CancellationToken cancellationToken = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = 1099,
            Currency = "eur",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
        };
        var service = new PaymentIntentService();
        return service.CreateAsync(options, cancellationToken: cancellationToken);
    }

    public async Task<Session> CreateAutoVotingSessionAsync(Server server, string successUrl, string cancelUrl, CancellationToken cancellationToken = default)
    {
        var paymentOperationDbSet = _dbContext.PaymentOperationDbSet;

        var priceOptions = new PriceListOptions
        {
            LookupKeys = new List<string> {
                "auto_vote"
            }
        };
        var priceService = new PriceService(_stripeClient);
        StripeList<Price> prices = await priceService.ListAsync(priceOptions, cancellationToken: cancellationToken);

        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Price = prices.Data[0].Id,
                    Quantity = 1
                }
            },
            Metadata =
            new Dictionary<string, string> {
                { "ServerId", $"{server.Id}" }
            },
            Mode = "subscription",
            SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{cancelUrl}?session_id={{CHECKOUT_SESSION_ID}}"
        };

        var service = new SessionService(_stripeClient);
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        var paymentOperationEntity = new PaymentOperation
        {
            State = PaymentOperationState.InProgress,
            PaymentProviderSessionId = session.Id
        };
        await paymentOperationDbSet.AddAsync(paymentOperationEntity, cancellationToken);

        return session;
    }

    public async Task<Stripe.BillingPortal.Session> CreatePortalSessionAsync(string returnUrl, string sessionId, CancellationToken cancellationToken = default)
    {
        // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
        // Typically this is stored alongside the authenticated user in your database.

        var checkoutService = new SessionService(_stripeClient);

        var checkoutSession = await checkoutService.GetAsync(sessionId, cancellationToken: cancellationToken);

        var options = new Stripe.BillingPortal.SessionCreateOptions

        {
            Customer = checkoutSession.CustomerId,

            ReturnUrl = returnUrl,
        };

        var service = new Stripe.BillingPortal.SessionService(_stripeClient);

        return await service.CreateAsync(options, cancellationToken: cancellationToken);
    }

    public async Task<bool> IsPaymentValidAsync(long operationId, CancellationToken cancellationToken)
    {
        if (!_temporaryOperationSessionMap.ContainsKey(operationId)) return false;
        var session = _temporaryOperationSessionMap[operationId];

        var service = new SessionService(_stripeClient);
        session = await service.GetAsync(session.Id, new SessionGetOptions()
        {
            Expand = new List<string> { "payment_intent" }
        }, cancellationToken: cancellationToken);

        if (session.PaymentStatus == "paid")
        {
            return true;
        }

        return false;
    }

    private async Task ValidatePaymentOperationAsync(PaymentOperation paymentOperation, Session session,
        CancellationToken cancellationToken)
    {
        if (session.Status == "complete")
        {
            paymentOperation.Payment = new StripePayment
            {
                StripeId = session.Id,
                Locale = session.Locale,
                Currency = session.Currency,
                CustomerEmail = session.CustomerEmail,
                CustomerId = session.CustomerId,
                StripePaymentIntentId = session.PaymentIntentId,
                AmountInCents = session.PaymentIntent.Amount,
                AutoVoteTasks = Enumerable.Empty<AutoVoteTask>(), // TODO
                CancellationReason = ConvertToCancellationReason(session.PaymentIntent.CancellationReason),
                StripeInvoiceId = session.PaymentIntent.InvoiceId,
                CanceledAt = session.PaymentIntent.CanceledAt,
                Status = ConvertToPaymentStatus(session.PaymentIntent.Status)
            };

            paymentOperation.State = PaymentOperationState.ForwardedToPaymentProvider;
        }
        else if (session.Status == "open")
        {
            paymentOperation.State = PaymentOperationState.InProgress;
        }
        else
        {
            paymentOperation.State = PaymentOperationState.Cancelled;
        }

        _dbContext.PaymentOperationDbSet.Update(paymentOperation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static StripePaymentCancellationReason ConvertToCancellationReason(string reason) => reason switch
    {
        "duplicate" => StripePaymentCancellationReason.Duplicate,
        "fraudulent" => StripePaymentCancellationReason.Fraudulent,
        "requested_by_customer" => StripePaymentCancellationReason.RequestedByCustomer,
        "abandoned" => StripePaymentCancellationReason.Abandoned,
        "failed_invoice" => StripePaymentCancellationReason.FailedInvoice,
        "void_invoice" => StripePaymentCancellationReason.VoidInvoice,
        "automatic" => StripePaymentCancellationReason.Automatic,
        _ => throw new ArgumentOutOfRangeException(nameof(reason))
    };

    private static StripePaymentStatus ConvertToPaymentStatus(string status)
    {
        return status switch
        {
            "canceled" => StripePaymentStatus.Canceled,
            "processing" => StripePaymentStatus.Processing,
            "requires_payment_method" => StripePaymentStatus.RequiresPaymentMethod,
            "requires_action" => StripePaymentStatus.RequiresAction,
            "requires_capture" => StripePaymentStatus.RequiresCapture,
            "requires_confirmation" => StripePaymentStatus.RequiresConfirmation,
            "succeeded" => StripePaymentStatus.Succeeded,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    async Task IPaymentService.RefreshPayments(CancellationToken cancellationToken)
    {
        var service = new SessionService(_stripeClient);
        var sessions = await service.ListAsync(cancellationToken: cancellationToken);

        foreach (var session in sessions)
        {
            // TODO
        }
    }
}