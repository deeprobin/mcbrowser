namespace MinecraftServerlist.InternalApi.Client.Repositories;

public abstract class BaseRepository
{
    protected HttpClient HttpClient { get; }

    internal BaseRepository(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }
}