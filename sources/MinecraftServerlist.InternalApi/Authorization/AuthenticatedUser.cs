using MinecraftServerlist.Data.Entities.Identity;
using System.Security.Principal;

namespace MinecraftServerlist.InternalApi.Authorization;

internal sealed class AuthenticatedUser : IIdentity
{
    internal AuthenticatedUser(Session session)
    {
        Session = session;
    }

    public Session Session { get; }

    public User User => Session.User;

    public string? AuthenticationType => "Basic";
    public bool IsAuthenticated => true;
    public string? Name => User.DisplayName;
}