namespace MinecraftServerlist.Ping;

internal static class BinaryWriterExtensions
{
    internal static void WritePacket(this BinaryWriter writer, Packet packet)
    {
        writer.Write7BitEncodedInt(packet.PacketLength);
        writer.Write7BitEncodedInt(packet.PacketId);
        writer.Write(packet.PacketContents);
    }
}