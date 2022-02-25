using Bogus;
using Microsoft.EntityFrameworkCore;

namespace MinecraftServerlist.Data.Development;

internal static class FakerExtensions
{
    internal static T RandomDatabaseEntry<T>(this Faker faker, DbContext dbContext) where T : class
    {
        var set = dbContext.Set<T>();
        if (!set.Any())
        {
            throw new Exception($"DbSet with type {typeof(T).Name} is not initialized");
        }

        var count = set.Count();
        var targetIndex = faker.Random.Int(0, count - 1);

        var targetItem = set.Skip(targetIndex).First();

        return targetItem;
    }
}