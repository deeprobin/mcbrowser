using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Consistency.Diagnostics;
using MinecraftServerlist.Data.Consistency.Fixes;
using MinecraftServerlist.Data.Infrastructure;

namespace MinecraftServerlist.Data.Consistency.Checks;

internal sealed class ServerDescriptionAvailableConsistencyCheck : ConsistencyCheck
{
    private readonly PostgresDbContext _dbContext;

    public ServerDescriptionAvailableConsistencyCheck(PostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task Run(CancellationToken cancellationToken = default)
    {
        var query = from server in _dbContext.ServerDbSet.Include(entity => entity.Descriptions)
                    where server.Descriptions == null || !server.Descriptions.Any()
                    select server;

        var queryServersCount = await query.CountAsync(cancellationToken);
        ProgressMax = (uint)queryServersCount;
        ProgressValue = 0;

        const int chunkSize = 64;
        for (var i = 0; i < queryServersCount; i += chunkSize)
        {
            var chunkedData = await query.Skip(i).Take(chunkSize).ToListAsync(cancellationToken);
            foreach (var diagnostic in
                     from server in chunkedData
                     let fix = new RemoveServerFix(_dbContext, server)
                     select new ConsistencyDiagnostic($"Server {server.Id} has no descriptions", fix: fix))
            {
                await ReportDiagnosticAsync(diagnostic, cancellationToken);
            }
            ProgressValue += chunkSize;
        }
    }
}