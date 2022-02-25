using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Services.Abstractions;

public interface IPingService
{
    public Task PingServerAsync(Server server, CancellationToken cancellationToken = default);
}