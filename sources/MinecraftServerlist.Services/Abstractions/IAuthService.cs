using MinecraftServerlist.Data.Entities.Identity;
using System.Net;

namespace MinecraftServerlist.Services.Abstractions;

public interface IAuthService
{
    public Task<User> RegisterAsync(string username, string password, string displayName, CancellationToken cancellationToken = default);

    public Task<Session> LoginAsync(string username, string password, IPAddress ipAddress, string userAgent, CancellationToken cancellationToken = default);

    public Task<User> GetUserBySessionAsync(byte[] sessionToken, CancellationToken cancellationToken = default);
}