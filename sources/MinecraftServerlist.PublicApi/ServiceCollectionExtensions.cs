using Microsoft.Extensions.DependencyInjection;

namespace MinecraftServerlist.PublicApi;

public static class ServiceCollectionExtensions
{
    public static void AddPublicApiServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
    }
}