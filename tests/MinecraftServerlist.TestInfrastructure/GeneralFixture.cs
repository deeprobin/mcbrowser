using Microsoft.Extensions.DependencyInjection;

namespace MinecraftServerlist.TestInfrastructure;

public class GeneralFixture : IDisposable
{
    public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

    public Lazy<IServiceProvider> ServiceProvider { get; }

    public GeneralFixture()
    {
        ServiceProvider = new Lazy<IServiceProvider>(() => ServiceCollection.BuildServiceProvider());
    }

    public void Dispose()
    {
    }
}