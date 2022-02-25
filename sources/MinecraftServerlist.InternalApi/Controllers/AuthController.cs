using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinecraftServerlist.InternalApi.Common.Bodies;
using MinecraftServerlist.InternalApi.Common.ResponseObjects;
using MinecraftServerlist.Services.Abstractions;
using System.Net;

namespace MinecraftServerlist.InternalApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/internal/[controller]/[action]")]
public sealed class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<LoginResponse> Login([FromBody] LoginBody body, CancellationToken cancellationToken)
    {
        if (body.Email == null)
        {
            throw new Exception("No email provided");
        }
        if (body.Password == null)

        {
            throw new Exception("No password provided");
        }

        var ipAddressString = HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"][0] ?? "0.0.0.0";
        var ipAddress = IPAddress.Parse(ipAddressString);

        var userAgent = HttpContext.Request.Headers["User-Agent"][0] ?? "";

        var session = await _authService.LoginAsync(body.Email, body.Password, ipAddress, userAgent, cancellationToken);

        return new LoginResponse
        {
            SessionToken = SessionTokenToString(session.TokenBytes)
        };
    }

    [HttpPost]
    public async Task<LoginResponse> Register([FromBody] RegisterBody body, CancellationToken cancellationToken)
    {
        if (body.Email == null)
        {
            throw new Exception("No email provided");
        }
        if (body.Password == null)

        {
            throw new Exception("No password provided");
        }

        var ipAddressString = HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"][0] ?? "0.0.0.0";
        var ipAddress = IPAddress.Parse(ipAddressString);

        var userAgent = HttpContext.Request.Headers["User-Agent"][0] ?? "";

        _ = await _authService.RegisterAsync(body.Email, body.Password, body.DisplayName ?? "Unnamed user", cancellationToken);
        var session = await _authService.LoginAsync(body.Email, body.Password, ipAddress, userAgent, cancellationToken);

        return new LoginResponse
        {
            SessionToken = SessionTokenToString(session.TokenBytes)
        };
    }

    private static string SessionTokenToString(byte[] token)
    {
        return Convert.ToBase64String(token);
    }

    private static byte[] StringToSessionToken(string token)
    {
        return Convert.FromBase64String(token);
    }
}