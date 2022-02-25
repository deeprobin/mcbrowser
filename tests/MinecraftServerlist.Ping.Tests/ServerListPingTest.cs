namespace MinecraftServerlist.Ping.Tests
{
    public class ServerListPingTest
    {
        [Fact]
        public async Task Test1()
        {
            var result = await ServerListPing.PingAsync("chaosolymp.de", 25565);
        }
    }
}