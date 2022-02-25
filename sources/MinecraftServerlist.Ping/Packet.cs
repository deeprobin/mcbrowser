namespace MinecraftServerlist.Ping;

internal struct Packet
{
    public int PacketLength;
    public int PacketId;
    public byte[] PacketContents;

    public Packet(int packetId, byte[] contents)
    {
        PacketId = packetId;
        PacketLength = sizeof(byte) + contents.Length;
        PacketContents = contents;
    }
}