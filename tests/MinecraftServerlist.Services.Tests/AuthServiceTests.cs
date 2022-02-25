using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Services.Extensions;

namespace MinecraftServerlist.Services.Tests;

public sealed class AuthServiceTests
{
    private readonly IServiceProvider _serviceProvider;

    private IAuthService AuthService => _serviceProvider.GetRequiredService<IAuthService>();
    private PostgresDbContext DbContext => _serviceProvider.GetRequiredService<PostgresDbContext>();

    public AuthServiceTests()
    {
        var serviceCollection = new ServiceCollection()
            .AddDbContext<PostgresDbContext>(options => options.UseInMemoryDatabase("TestDatabase"))
            .AddLogging()
            .AddServices();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}