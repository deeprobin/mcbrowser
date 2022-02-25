namespace MinecraftServerlist.Services.Abstractions;

public interface IServerMonitoringService
{
    public void StoreServerStatistic(int serverId, bool online, int onlinePlayers, DateTime timestamp);
}