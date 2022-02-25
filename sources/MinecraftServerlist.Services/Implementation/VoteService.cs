using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Votifier;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class VoteService : IVoteService
{
    private readonly PostgresDbContext _dbContext;

    public VoteService(PostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServerVoting> SubmitVoteAsync(Server server, User user, string minecraftUser, bool sendVotifierMessage, CancellationToken cancellationToken = default)
    {
        var votingEntity = new ServerVoting
        {
            User = user,
            CreatedAt = DateTime.Now,
            MinecraftUsername = minecraftUser,
            Server = server
        };
        var dbResult = await _dbContext.ServerVotingDbSet.AddAsync(votingEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (sendVotifierMessage)
        {
            await SendVotifierMessageAsync(server, user, minecraftUser, cancellationToken);
        }

        return dbResult.Entity;
    }

    public async Task<bool> SendVotifierMessageAsync(Server server, User user, string minecraftUser, CancellationToken cancellationToken = default)
    {
        var dateTime = DateTime.Now;
        DateTimeOffset dateTimeOffset = dateTime;
        var unixTime = dateTimeOffset.ToUnixTimeSeconds();

        try
        {
            var votifierHost = server.VotifierAddress ?? server.ServerAddress;
            var votifierPort = server.VotifierPort ?? 8192;
            var votifierToken = server.VotifierToken ?? "anonymous";

            await Voter.SubmitVote(votifierHost, votifierPort, minecraftUser, votifierToken, unixTime,
                cancellationToken);
        }
        catch (VotifierException)
        {
            // Vote failed
            return false;
        }
        catch (IOException ex)
        {
            // TODO: Implement Backup Handler if server is not reachable (try again when server is back)
        }

        return true;
    }
}