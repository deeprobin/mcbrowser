using Microsoft.Extensions.Logging;
using MinecraftServerlist.Common.Attributes;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Ping;
using MinecraftServerlist.Services.Abstractions;
using System.Net.Sockets;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class PingService : IPingService
{
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<PingService> _logger;

    public PingService(PostgresDbContext dbContext, ILogger<PingService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Intrinsic]
    public async Task PingServerAsync(Server server, CancellationToken cancellationToken = default)
    {
        var pingEntity = new ServerPing
        {
            Server = server,
            CreatedAt = DateTime.Now,
            Online = false
        };
        try
        {
            var serverListEntry =
                await ServerListPing.PingAsync(server.ServerAddress, server.ServerPort, cancellationToken);

            pingEntity.Online = true;
            pingEntity.MaxPlayers = serverListEntry.PlayersElement?.MaxPlayers;
            pingEntity.OnlinePlayers = serverListEntry.PlayersElement?.OnlinePlayers;
            pingEntity.VersionName = serverListEntry.Version?.Name;
            pingEntity.VersionProtocol = serverListEntry.Version?.Protocol;
            pingEntity.MessageOfTheDay = serverListEntry.Description?.ToString();
        }
        catch (Exception ex) when (ex is IOException or SocketException)
        {
            _logger.LogInformation("Ping to Server #{Id} failed", server.Id);
        }

        await _dbContext.AddAsync(pingEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}