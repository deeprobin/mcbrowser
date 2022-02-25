using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.InternalApi.Common.ResponseObjects;

namespace MinecraftServerlist.InternalApi;

internal static class Mapper
{
    internal static Task<ServerResponse> MapToServerResponseAsync(Server server)
    {
        var description = server.Descriptions?.FirstOrDefault();
        return Task.FromResult(new ServerResponse
        {
            Id = server.Id,
            Title = description?.Title,
            ShortDescription = description?.ShortDescription,
            OnlinePlayers = 0,
            MaxPlayers = 0,
            VoteCount = server.ReceivedVotings?.Count()
        });
    }

    internal static Task<AdvancedServerResponse> MapToAdvancedServerResponseAsync(Server server)
    {
        var description = server.Descriptions?.FirstOrDefault();
        return Task.FromResult(new AdvancedServerResponse
        {
            Id = server.Id,
            Title = description?.Title,
            ShortDescription = description?.ShortDescription,
            OnlinePlayers = 0,
            MaxPlayers = 0,
            VoteCount = server.ReceivedVotings?.Count(),
            DiscordInvitationId = description?.DiscordInvitationId,
            LongDescription = description?.LongDescription,
            OwnerId = server.OwnerId,
            ServerAddress = server.ServerAddress,
            ServerPort = server.ServerPort,
            TeamspeakAddress = description?.TeamspeakAddress,
            Website = description?.Website
        });
    }
}