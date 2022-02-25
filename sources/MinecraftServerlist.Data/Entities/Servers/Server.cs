using MinecraftServerlist.Data.Entities.Financial;
using MinecraftServerlist.Data.Entities.Identity;

namespace MinecraftServerlist.Data.Entities.Servers;

public sealed record Server
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public User Owner { get; set; } = default!;

    public string ServerAddress { get; set; } = default!;

    public ushort ServerPort { get; set; }

    public ServerState ServerState { get; set; }

    public IEnumerable<ServerDescription>? Descriptions { get; set; } = default;

    public IEnumerable<ServerVoting>? ReceivedVotings { get; set; } = default;

    public IEnumerable<ServerPing>? ReceivedPings { get; set; } = default;

    public IEnumerable<AutoVoteTask>? AutoVoteTasks { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public string? VotifierAddress { get; set; }

    public ushort? VotifierPort { get; set; }

    public string? VotifierToken { get; set; }
}