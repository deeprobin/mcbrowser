using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace MinecraftServerlist.TestInfrastructure;

public sealed class IntegrationFixture
{
    private readonly WebApplicationFactory<Program> _applicationFactory;

    public TestServer TestServer { get; }
    public IServiceProvider ServiceProvider { get; }

    public IntegrationFixture()
    {
        _applicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // ... Configure test services
            });

        ServiceProvider = _applicationFactory.Services;
        TestServer = _applicationFactory.Server;
    }

    public HttpClient CreateClient() => _applicationFactory.CreateClient();

    public HttpClient CreateClient(WebApplicationFactoryClientOptions options) => _applicationFactory.CreateClient(options);
}