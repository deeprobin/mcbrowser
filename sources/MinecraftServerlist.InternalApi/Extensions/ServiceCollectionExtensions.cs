using Microsoft.Extensions.DependencyInjection;

namespace MinecraftServerlist.InternalApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInternalApiServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
    }
}