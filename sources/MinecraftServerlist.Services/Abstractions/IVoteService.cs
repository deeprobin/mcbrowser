using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Services.Abstractions;

public interface IVoteService
{
    public Task<ServerVoting> SubmitVoteAsync(Server server, User user, string minecraftUser, bool sendVotifierMessage = true, CancellationToken cancellationToken = default);

    public Task<bool> SendVotifierMessageAsync(Server server, User user, string minecraftUser, CancellationToken cancellationToken = default);
}