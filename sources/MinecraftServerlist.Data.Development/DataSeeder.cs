using Bogus;
using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Infrastructure;
using System.Diagnostics;

namespace MinecraftServerlist.Data.Development;

public static partial class DataSeeder
{
    public static void SeedData(PostgresDbContext dbContext)
    {
        Seed(dbContext, UserFaker, 100);
        Seed(dbContext, GetServerFaker(dbContext), 100);
        Seed(dbContext, GetServerDescriptionFaker(dbContext), 1000);
        //Seed(dbContext, GetServerVotingFaker(dbContext), 10_000);

        CreateDevelopmentUser(dbContext);
    }

    private static void Seed<T>(DbContext dbContext, Faker<T> faker, int n) where T : class
    {
        var dbSet = dbContext.Set<T>();

        var generatedValues = faker.Generate(n);
        dbSet.AddRange(generatedValues);

        var rowsAffected = dbContext.SaveChanges();
        Debug.Assert(n == rowsAffected);
    }

    private static void CreateDevelopmentUser(PostgresDbContext dbContext)
    {
        var dbSet = dbContext.UserDbSet;
        var firstUser = dbSet.First();

        firstUser.DisplayName = "Administrator";
        firstUser.MailAddress = "admin@mcserver.com";
        firstUser.UserRole = UserRole.Admin;

        dbSet.Update(firstUser);
        dbContext.SaveChanges();
    }
}