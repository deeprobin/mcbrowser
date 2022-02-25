using MinecraftServerlist.InternalApi.Common.Bodies;
using System.Net.Http.Json;

namespace MinecraftServerlist.InternalApi.Client.Repositories;

public sealed class PaymentRepository : BaseRepository
{
    internal PaymentRepository(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task ActivateAutoVoteTask(StripeBillingRequest request, CancellationToken cancellationToken = default)
    {
        const string url = "Payment/ActivateAutoVoteTask";

        var response = await HttpClient.PostAsync(url, JsonContent.Create(request), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Cannot ActivateAutoVoteTask - Status Code {response.StatusCode}");
        }
    }
}