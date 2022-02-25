using Microsoft.Extensions.DependencyInjection;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Services.Implementation;

namespace MinecraftServerlist.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
                // Scoped Services
                // All services that use scoped services or
                // have a good reason to be scoped (including the use of a DbContext).
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IPingService, PingService>()
                .AddScoped<IServerService, ServerService>()
                .AddScoped<IVoteService, VoteService>()
                .AddScoped<IPaymentService, PaymentService>()
                //.AddScoped<IServerMonitoringService, ServerMonitoringService>()
                //.AddScoped<IMailService, MailService>()

                // Singleton Services
                // All services for which it is sufficient to instantiate them
                // once (Caution: only if they do not consume scoped services).
                .AddSingleton<IRuntimeService, RuntimeService>()
                .AddSingleton<IMetricsService, MetricsService>()
                .AddSingleton<IAdminSuggestionService, AdminSuggestionService>()

            // Hosted Services
            // All worker services
            //.AddHostedService<PingHostedService>()
            ;
    }
}