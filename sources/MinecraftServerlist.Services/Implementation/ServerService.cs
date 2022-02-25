using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class ServerService : IServerService
{
    private readonly ILogger<ServerService> _logger;
    private readonly PostgresDbContext _dbContext;

    public ServerService(PostgresDbContext dbContext, ILogger<ServerService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Server> CreateServerAsync(Server server, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.ServerDbSet.AddAsync(server, cancellationToken);
        var entity = result.Entity;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created server with id {Id}", entity.Id);
        return entity;
    }

    public async Task<Server> EnableServerAsync(Server server, CancellationToken cancellationToken = default)
    {
        server.ServerState = ServerState.Enabled;
        _dbContext.ServerDbSet.Update(server);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return server;
    }

    public IAsyncEnumerable<Server> GetTopServersAsync(int amount, int skip = 0,
        CancellationToken cancellationToken = default)
    {
        var query = QueryServersWithMostVotes(GetVotingMonthStart());
        return query.Skip(skip).Take(amount).ToAsyncEnumerable();
    }

    internal static DateTime GetVotingMonthStart()
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, 1, 0, 0, 0);
    }

    internal IQueryable<Server> QueryServersWithMostVotes(DateTime after) => from server in _dbContext.ServerDbSet.Include(entity => entity.ReceivedVotings).Include(entity => entity.Descriptions)
                                                                             where server.ServerState == ServerState.Enabled
                                                                             let voteCount = (
                                                                                 from voting in _dbContext.ServerVotingDbSet
                                                                                 where voting.CreatedAt <= after
                                                                                     && voting.Server == server
                                                                                 select server
                                                                                 ).ToList().Count
                                                                             // Order by vote count
                                                                             orderby voteCount descending,
                                                                                 // Then by server creation date
                                                                                 server.CreatedAt descending
                                                                             select server;

    public IAsyncEnumerable<Server> GetNewestServersAsync(int amount, int skip = 0, CancellationToken cancellationToken = default) =>
        QueryNewestServers().Skip(skip).Take(amount).AsAsyncEnumerable();

    public IAsyncEnumerable<Server> GetPendingServersAsync(int amount, int skip = 0,
        CancellationToken cancellationToken = default) =>
        QueryPendingServers().Skip(skip).Take(amount).AsAsyncEnumerable();

    public Task<Server?> GetServerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var query = from server in _dbContext.ServerDbSet.Include(entity => entity.Descriptions)
                    where server.Id == id
                    select server;

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<int> CountServersAsync(CancellationToken cancellationToken = default)
    {
        return await QueryEnabledServers().CountAsync(cancellationToken);
    }

    private IQueryable<Server> QueryEnabledServers() =>
        from server in _dbContext.ServerDbSet.Include(entity => entity.Descriptions)
        select server;

    private IQueryable<Server> QueryPendingServers() =>
        from server in _dbContext.ServerDbSet.Include(entity => entity.Descriptions)
        where server.ServerState == ServerState.PendingActivation
        orderby server.CreatedAt
        select server;

    private IQueryable<Server> QueryNewestServers() =>
        from server in _dbContext.ServerDbSet.Include(entity => entity.Descriptions)
        where server.ServerState == ServerState.Enabled
        orderby server.CreatedAt
        select server;
}