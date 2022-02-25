using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;

namespace MinecraftServerlist.Ping;

public static class ServerListPing
{
    public static async Task<ServerListEntry> PingAsync(string hostname, int port, CancellationToken cancellationToken = default)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(hostname, port, cancellationToken);

        Debug.Assert(tcpClient.Connected);
        await using var networkStream = tcpClient.GetStream();
        await using var writer = new BinaryWriter(networkStream);

        var handshakePacket = CreateHandshakePacket(47, hostname, (short)port, 1);
        writer.WritePacket(handshakePacket);

        var statusRequestPacket = CreateHandshakeFollowupRequestPacket();
        writer.WritePacket(statusRequestPacket);

        writer.Flush();
        await networkStream.FlushAsync(cancellationToken);

        using var reader = new BinaryReader(networkStream);
        var length = reader.Read7BitEncodedInt();
        var packetId = reader.Read7BitEncodedInt();
        //var jsonLength = reader.Read7BitEncodedInt();

        Debug.Assert(packetId == 0x00);
        // Debug.Assert(length > jsonLength);

        var json = reader.ReadString();
        var serverListEntry = JsonSerializer.Deserialize<ServerListEntry>(json)!;

        return serverListEntry;
    }

    private static Packet CreateHandshakePacket(int protocolVersion, string hostname, short port, int nextState)
    {
        // Send "Handshake" packet
        // http://wiki.vg/Server_List_Ping#Ping_Process
        var memoryStream = new MemoryStream();
        var packetWriter = new BinaryWriter(memoryStream);

        // Write protocol version
        packetWriter.Write7BitEncodedInt(protocolVersion);

        // Write server address
        packetWriter.Write(hostname);

        // Write port
        packetWriter.Write(port);

        // Write "Next state" (1 = status; 2 = login)
        packetWriter.Write7BitEncodedInt(nextState);

        return new Packet(0x00, memoryStream.ToArray());
    }

    private static Packet CreateHandshakeFollowupRequestPacket()
    {
        // Send "Request" packet
        // http://wiki.vg/Server_List_Ping#Ping_Process
        return new Packet(0x00, Array.Empty<byte>());
    }
}