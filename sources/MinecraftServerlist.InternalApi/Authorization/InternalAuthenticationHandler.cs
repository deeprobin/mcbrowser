using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Services.Exceptions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace MinecraftServerlist.InternalApi.Authorization;

internal sealed class InternalAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAuthService _authService;

    public InternalAuthenticationHandler(IAuthService authService, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _authService = authService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Response.Headers.Add("WWW-Authenticate", "Basic");

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Authorization header missing.");
        }

        // Get authorization key
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        var authHeaderRegex = new Regex(@"Basic (.*)");

        if (!authHeaderRegex.IsMatch(authorizationHeader))
        {
            return AuthenticateResult.Fail("Authorization code not formatted properly.");
        }

        var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
        var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
        var authUsername = authSplit[0];
        var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

        var ipAddressString = Request.Headers["HTTP_X_FORWARDED_FOR"][0] ?? "0.0.0.0";
        var ipAddress = IPAddress.Parse(ipAddressString);

        var userAgent = Request.Headers["User-Agent"][0] ?? "";

        try
        {
            var session = await _authService.LoginAsync(authUsername, authPassword, ipAddress, userAgent);

            var authenticatedUser = new AuthenticatedUser(session);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
        catch (AuthenticationException)
        {
            return AuthenticateResult.Fail("Auth failed");
        }
    }
}