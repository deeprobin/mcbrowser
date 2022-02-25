using Bogus;
using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.Development;

public static partial class DataSeeder
{
    private static Faker<Server> GetServerFaker(DbContext dbContext) => new Faker<Server>()
        .RuleFor(entity => entity.Id, f => f.IndexFaker + 1)
        .RuleFor(entity => entity.Owner, f => f.RandomDatabaseEntry<User>(dbContext))
        .RuleFor(entity => entity.CreatedAt, f => f.Date.Past())
        .RuleFor(entity => entity.ServerAddress, f => f.Internet.DomainName())
        .RuleFor(entity => entity.ServerPort, f => f.Random.UShort())
        .RuleFor(entity => entity.VotifierAddress, f => f.Internet.DomainName().OrNull(f))
        .RuleFor(entity => entity.VotifierPort, f => f.Random.UShort().OrNull(f, 0.7f))
        .RuleFor(entity => entity.ServerState, f => f.PickRandom<ServerState>())
        .RuleFor(entity => entity.VotifierToken, f => f.Random.AlphaNumeric(32).OrNull(f, 0.9f));
}