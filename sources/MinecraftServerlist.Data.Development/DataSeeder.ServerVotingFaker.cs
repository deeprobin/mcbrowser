using Bogus;
using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.Development;

public static partial class DataSeeder
{
    private static Faker<ServerVoting> GetServerVotingFaker(DbContext dbContext) => new Faker<ServerVoting>()
        .RuleFor(entity => entity.Id, f => f.IndexFaker + 1)
        .RuleFor(entity => entity.User, f => f.RandomDatabaseEntry<User>(dbContext))
        .RuleFor(entity => entity.Server, f => f.RandomDatabaseEntry<Server>(dbContext))
        .RuleFor(entity => entity.CreatedAt, f => f.Date.Past())
        .RuleFor(entity => entity.MinecraftUsername, f => f.Random.String(3, 16, 'A', 'Z'));
}