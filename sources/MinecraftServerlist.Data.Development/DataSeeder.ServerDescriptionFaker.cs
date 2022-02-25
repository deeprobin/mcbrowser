using System.Globalization;
using Bogus;
using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.Development;

public static partial class DataSeeder
{
    private static readonly string[] Locales = CultureInfo.GetCultures(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures).Select(c => c.IetfLanguageTag).ToArray();

    private static Faker<ServerDescription> GetServerDescriptionFaker(DbContext dbContext) =>
        new Faker<ServerDescription>()
            .CustomInstantiator(f =>
            {
                var server = f.RandomDatabaseEntry<Server>(dbContext);
                do
                {
                    f.Locale = f.PickRandom(Locales);
                } while (dbContext.Set<Server>().Include(s => s.Descriptions)
                         .Where(s => s.Id == server.Id && s.Descriptions != null)
                         .SelectMany(s => s.Descriptions!)
                         .Any(desc => desc.Culture == f.Locale));

                var description = new ServerDescription
                {
                    Culture = f.Locale,
                    Server = server
                };

                return description;
            })
            .RuleFor(entity => entity.Id, f => f.IndexFaker + 1)
            .RuleFor(entity => entity.Server, f => f.RandomDatabaseEntry<Server>(dbContext))
            .RuleFor(entity => entity.Culture, f => f.Locale)
            .RuleFor(entity => entity.LongDescription, f => f.Lorem.Paragraphs(10, 20))
            .RuleFor(entity => entity.ShortDescription, f => f.Company.CatchPhrase())
            .RuleFor(entity => entity.Title, f => f.Company.CompanyName())
            .RuleFor(entity => entity.DiscordInvitationId, f => f.Random.AlphaNumeric(6).OrNull(f))
            .RuleFor(entity => entity.TeamspeakAddress, f => f.Internet.DomainName().OrNull(f))
            .RuleFor(entity => entity.Website, f => f.Internet.DomainName());
}