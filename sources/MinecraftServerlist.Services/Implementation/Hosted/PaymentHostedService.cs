using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinecraftServerlist.Services.Abstractions;

namespace MinecraftServerlist.Services.Implementation.Hosted;

internal sealed class PaymentHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            await paymentService.RefreshPayments(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}