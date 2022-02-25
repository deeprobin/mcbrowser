using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinecraftServerlist.Services.Abstractions;
using Stripe;

namespace MinecraftServerlist.InternalApi.Controllers;

[ApiController]
[Route("api/internal/[controller]/[action]")]
public sealed class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IServerService _serverService;
    private readonly ILogger<IPaymentService> _logger;

    public PaymentController(IPaymentService paymentService, IServerService serverService, ILogger<IPaymentService> logger)
    {
        _paymentService = paymentService;
        _serverService = serverService;
        _logger = logger;
    }

    [HttpPost("{serverId:int}")]
    public async Task<ActionResult> CreateAutoVoteSession([FromRoute] int serverId)
    {
        var host = Request.Host;
        var baseUrl = $"https://{Request.Host.Host}";
        if (host.Port is { } port)
        {
            baseUrl += $":{port}";
        }

        var server = await _serverService.GetServerByIdAsync(serverId);
        if (server is null)
        {
            return BadRequest("Invalid server id");
        }

        var session = await _paymentService
            .CreateAutoVotingSessionAsync(server, $"{baseUrl}/checkout/success", $"{baseUrl}/checkout/cancel");

        return Redirect(session.Url);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePortalSession()
    {
        var sessionId = Request.Form["session_id"];
        var session = await _paymentService.CreatePortalSessionAsync("https://google.com", sessionId);

        return Redirect(session.Url);
    }

    [HttpPost]
    public async Task<IActionResult> StripeWebHook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            _logger.LogTrace("Incoming Stripe-WebHook message: {EventJson}", json);
            var stripeEvent = EventUtility.ParseEvent(json);
            await _paymentService.ReceiveWebHookAsync(stripeEvent);

            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
    }

    /*

    [HttpPost]
    public async Task ActivateAutoVoteTask(StripeBillingRequest request)
    {
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromQuery] int serverId)
    {
        try
        {
            var host = Request.Host;
            var baseUrl = $"https://{Request.Host.Host}";
            if (host.Port is { } port)
            {
                baseUrl += $":{port}";
            }

            var server = await _serverService.GetServerByIdAsync(serverId);
            if (server is null)
            {
                return BadRequest("Invalid server id");
            }

            var session = await _paymentService
                .CreateAutoVotingSessionAsync(server, 30, $"{baseUrl}/checkout/success", $"{baseUrl}/checkout/cancel");

            return Redirect(session.Url);
        }
        catch (StripeException e)
        {
            Console.WriteLine(e.StripeError.Message);
            return BadRequest(e.StripeError.Message);
        }
    }

    [HttpGet("checkout-session")]
    public async Task<IActionResult> CheckoutSession(string sessionId)
    {
        var service = new SessionService(this.client);
        var session = await service.GetAsync(sessionId);
        return Ok(session);
    }

    [HttpPost("customer-portal")]
    public async Task<IActionResult> CustomerPortal(string sessionId)
    {
        // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
        // Typically this is stored alongside the authenticated user in your database.
        var checkoutService = new SessionService(this.client);
        var checkoutSession = await checkoutService.GetAsync(sessionId);

        // This is the URL to which your customer will return after
        // they are done managing billing in the Customer Portal.
        var returnUrl = this.options.Value.Domain;

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = checkoutSession.CustomerId,
            ReturnUrl = returnUrl,
        };
        var service = new Stripe.BillingPortal.SessionService(this.client);
        var session = await service.CreateAsync(options);

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                this.options.Value.WebhookSecret
            );
            Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something failed {e}");
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            Console.WriteLine($"Session ID: {session.Id}");
            // Take some action based on session.
        }

        return Ok();
    }*/
}