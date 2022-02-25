using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Services.Extensions;

namespace MinecraftServerlist.Services.Tests;

public sealed class ServerServiceTests : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly User _dummyUser;

    private IServerService ServerService => _serviceProvider.GetRequiredService<IServerService>();

    private IVoteService VoteService => _serviceProvider.GetRequiredService<IVoteService>();

    private PostgresDbContext DbContext => _serviceProvider.GetRequiredService<PostgresDbContext>();

    public ServerServiceTests()
    {
        var serviceCollection = new ServiceCollection()
            .AddDbContext<PostgresDbContext>(options => options.UseInMemoryDatabase("TestDatabase"))
            .AddLogging()
            .AddServices();

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var dummyUserEntry = DbContext.UserDbSet.Add(new User
        {
            MailAddress = "test@dev.mcbrowser.com",
            DisplayName = "Test User",
            HashedPassword = new byte[256],
            UserState = UserState.Enabled
        });

        _dummyUser = dummyUserEntry.Entity;
    }

    [Fact]
    public async Task TestCreateServer()
    {
        var server = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Default,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25565
        });

        var count = await DbContext.ServerDbSet.CountAsync();
        Assert.Equal(1, count);

        var firstEntry = await DbContext.ServerDbSet.FirstAsync();
        Assert.Equal(server, firstEntry);
    }

    [Fact]
    public async Task TestEnableServer()
    {
        var server = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Default,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25565
        });

        server = await ServerService.EnableServerAsync(server);

        var firstEntry = await DbContext.ServerDbSet.FirstAsync();
        Assert.Equal(server, firstEntry);
    }

    [Fact]
    public async Task TestGetNewestServers()
    {
        var firstServer = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Enabled,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25565,
            CreatedAt = new DateTime(2000, 1, 1, 1, 1, 1)
        });

        // This server is not enabled, so GetNewestServersAsync must not return it
        await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Default,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25566,
            CreatedAt = new DateTime(2002, 2, 1, 1, 1, 1)
        });

        var thirdServer = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Enabled,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25567,
            CreatedAt = new DateTime(2002, 2, 1, 1, 1, 2)
        });

        var entries = await ServerService.GetNewestServersAsync(100).ToArrayAsync();
        Assert.Equal(2, entries.Length);

        Assert.Equal(firstServer, entries[0]);
        Assert.Equal(thirdServer, entries[1]);
    }

    [Fact]
    public async Task TestGetPendingServers()
    {
        var firstServer = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.PendingActivation,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25565,
            CreatedAt = new DateTime(2000, 1, 1, 1, 1, 1)
        });

        var secondServer = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.PendingActivation,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25566,
            CreatedAt = new DateTime(2002, 2, 1, 1, 1, 1)
        });

        // This server is not pending, so GetPendingServersAsync must not return it
        await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Enabled,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25567,
            CreatedAt = new DateTime(2002, 2, 1, 1, 1, 2)
        });

        var entries = await ServerService.GetPendingServersAsync(100).ToArrayAsync();
        Assert.Equal(2, entries.Length);

        Assert.Equal(firstServer, entries[0]);
        Assert.Equal(secondServer, entries[1]);
    }

    [Fact]
    public async Task TestGetTopServers()
    {
        var firstServer = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Enabled,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25565,
            CreatedAt = new DateTime(2000, 1, 1, 1, 1, 1)
        });

        // This server is not enabled, so GetTopServersAsync must not return it
        await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Default,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25566,
            CreatedAt = new DateTime(2002, 2, 1, 1, 1, 1)
        });

        var thirdServer = await ServerService.CreateServerAsync(new Server
        {
            Owner = _dummyUser,
            ServerState = ServerState.Enabled,
            ServerAddress = "dev.mcbrowser.com",
            ServerPort = 25567,
            CreatedAt = new DateTime(2002, 2, 1, 1, 1, 2)
        });

        await VoteService.SubmitVoteAsync(thirdServer, _dummyUser, "Notch", false);

        var entries = await ServerService.GetTopServersAsync(100).ToArrayAsync();
        Assert.Equal(2, entries.Length);

        Assert.Equal(thirdServer, entries[0]);
        Assert.Equal(firstServer, entries[1]);
    }

    public void Dispose()
    {
        var dbSet = DbContext.ServerDbSet;
        dbSet.RemoveRange(dbSet);

        DbContext.SaveChanges();
    }
}