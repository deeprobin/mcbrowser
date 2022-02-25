using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServerlist.InternalApi.Client;
using MinecraftServerlist.Web.Data;

namespace MinecraftServerlist.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection MapBlazorFrontend(this IServiceCollection serviceCollection, string baseAddress)
    {
        serviceCollection.AddRazorPages();

        serviceCollection.AddServerSideBlazor();
        serviceCollection.AddSingleton<WeatherForecastService>();

        serviceCollection.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });
        serviceCollection.AddScoped(_ =>
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress +  "api/internal/") };
            var internalClient = new InternalClient(httpClient);
            return internalClient;
        });

        return serviceCollection;
    }
}