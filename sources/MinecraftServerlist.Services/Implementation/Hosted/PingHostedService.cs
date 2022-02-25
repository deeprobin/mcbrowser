using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;

namespace MinecraftServerlist.Services.Implementation.Hosted;

internal sealed class PingHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private const int PingPriorityPages = 2;

    public PingHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var unpingedServers = await QueryUnpingedServers(scope).Take(10).ToListAsync(cancellationToken);
            var priorityServers = await QueryPriorityServers(scope).ToListAsync(cancellationToken);
            var longAgoServers = await QueryPingLongAgoServers(scope)
                .Take(10)
                .OrderBy(server => priorityServers.Contains(server))
                .ToListAsync(cancellationToken);

            var workQueue = unpingedServers
                .Concat(longAgoServers);

            var pingService = scope.ServiceProvider.GetRequiredService<IPingService>();
            var workTasks = workQueue
                .Select(server => pingService.PingServerAsync(server, cancellationToken));
            await Task.WhenAll(workTasks);
        }
    }

    private static IQueryable<Server> QueryUnpingedServers(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        return from server in dbContext.ServerDbSet
               where server.ReceivedPings == null
                     || !server.ReceivedPings.Any()
               select server;
    }

    private static IQueryable<Server> QueryPriorityServers(IServiceScope scope)
    {
        var serverService = (ServerService)scope.ServiceProvider.GetRequiredService<IServerService>();
        return serverService.QueryServersWithMostVotes(ServerService.GetVotingMonthStart()).Take(PingPriorityPages * 30);
    }

    private static IQueryable<Server> QueryPingLongAgoServers(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        return from server in dbContext.ServerDbSet.Include(server => server.ReceivedPings)
               let lastPing = server.ReceivedPings!.LastOrDefault()
               where lastPing != null
               orderby lastPing.CreatedAt descending
               select server;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}