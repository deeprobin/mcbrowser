using MinecraftServerlist.InternalApi.Common.ResponseObjects;
using System.Text.Json;

namespace MinecraftServerlist.InternalApi.Client.Repositories;

public sealed class ServerRepository : BaseRepository
{
    internal ServerRepository(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IEnumerable<ServerResponse>?> GetTopServersAsync(int amount, int skip, CancellationToken cancellationToken = default)
    {
        var url = $"Server/GetTopServers?amount={amount}&skip={skip}";

        await using var stream = await HttpClient.GetStreamAsync(url, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<ServerResponse>>(stream, cancellationToken: cancellationToken);

        return result;
    }

    public async Task<IEnumerable<ServerResponse>?> GetNewestServersAsync(int amount, int skip, CancellationToken cancellationToken = default)
    {
        var url = $"Server/GetNewestServers?amount={amount}&skip={skip}";

        await using var stream = await HttpClient.GetStreamAsync(url, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<ServerResponse>>(stream, cancellationToken: cancellationToken);

        return result;
    }

    public async Task<AdvancedServerResponse?> GetServerAsync(int id, CancellationToken cancellationToken = default)
    {
        var url = $"Server/GetServer?id={id}";

        await using var stream = await HttpClient.GetStreamAsync(url, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<AdvancedServerResponse>(stream, cancellationToken: cancellationToken);

        return result;
    }

    public async ValueTask<int?> GetCountAsync(CancellationToken cancellationToken = default)
    {
        const string url = "Server/CountServers";

        await using var stream = await HttpClient.GetStreamAsync(url, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<int>(stream, cancellationToken: cancellationToken);

        return result;
    }
}