using Microsoft.AspNetCore.Mvc;
using MinecraftServerlist.InternalApi.Common.Bodies;
using MinecraftServerlist.Services.Abstractions;

namespace MinecraftServerlist.InternalApi.Controllers;

[ApiController]
[Route("api/internal/[controller]/[action]")]
public sealed class CheckoutController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IServerService _serverService;

    public CheckoutController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateAutoVoteSession([FromQuery] int serverId)
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

    [HttpGet]
    public async Task<PaymentIntentBody> AutoVoteIntent([FromQuery] int serverId)
    {
        var paymentIntent = await _paymentService.CreateAutoVotePaymentIntentAsync();
        return new PaymentIntentBody
        {
            ClientSecret = paymentIntent.ClientSecret
        };
    }

    [HttpPost]
    public async Task<ActionResult> AutoVote([FromQuery] int serverId)
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
    public async Task<ActionResult> TryPaymentValidation()
    {
        return Ok();
    }
}