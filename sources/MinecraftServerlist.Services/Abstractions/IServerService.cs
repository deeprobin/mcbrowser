using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Services.Abstractions;

public interface IServerService
{
    public Task<Server> CreateServerAsync(Server server, CancellationToken cancellationToken = default);

    public Task<Server> EnableServerAsync(Server server, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Server> GetTopServersAsync(int amount, int skip = 0, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Server> GetNewestServersAsync(int amount, int skip = 0, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Server> GetPendingServersAsync(int amount, int skip = 0, CancellationToken cancellationToken = default);

    public ValueTask<int> CountServersAsync(CancellationToken cancellationToken = default);

    public Task<Server?> GetServerByIdAsync(int id, CancellationToken cancellationToken = default);
}