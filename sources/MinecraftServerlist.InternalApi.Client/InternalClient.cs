using MinecraftServerlist.InternalApi.Client.Repositories;

namespace MinecraftServerlist.InternalApi.Client;

public sealed class InternalClient
{
    public InternalClient(HttpClient httpClient)
    {
        ServerRepository = new ServerRepository(httpClient);
        PaymentRepository = new PaymentRepository(httpClient);
    }

    public ServerRepository ServerRepository { get; }

    public PaymentRepository PaymentRepository { get; }
}