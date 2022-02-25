using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;

namespace MinecraftServerlist.Data.Consistency.Fixes;

internal sealed class RemoveServerFix : IConsistencyFix
{
    private readonly PostgresDbContext _dbContext;
    private readonly Server _server;

    public RemoveServerFix(PostgresDbContext dbContext, Server server)
    {
        _dbContext = dbContext;
        _server = server;
    }

    public Task ApplyFixAsync(CancellationToken cancellationToken = default)
    {
        _dbContext.ServerDbSet.Remove(_server);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}