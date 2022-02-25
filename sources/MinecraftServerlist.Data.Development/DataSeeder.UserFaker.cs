using Bogus;
using MinecraftServerlist.Data.Entities.Identity;

namespace MinecraftServerlist.Data.Development;

public static partial class DataSeeder
{
    private static Faker<User> UserFaker => new Faker<User>()
        .RuleFor(entity => entity.Id, f => f.IndexFaker + 1)
        .RuleFor(entity => entity.CreatedAt, f => f.Date.Past())
        .RuleFor(entity => entity.DisplayName, f => f.Name.FullName())
        .RuleFor(entity => entity.HashedPassword, new byte[256])
        .RuleFor(entity => entity.MailAddress, f => f.Internet.Email())
        .RuleFor(entity => entity.LastUpdatedAt, DateTime.Now)
        .RuleFor(entity => entity.UserState, f => f.PickRandom<UserState>());
}