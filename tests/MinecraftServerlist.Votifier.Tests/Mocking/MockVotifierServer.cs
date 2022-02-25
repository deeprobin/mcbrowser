using System.Buffers.Binary;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MinecraftServerlist.Votifier.Tests.Mocking;

internal sealed class MockVotifierServer
{
    private readonly TcpListener _tcpListener;

    public string Token { get; set; } = "";

    public bool SendInvalidHeader { get; set; }

    public bool SendInvalidVersion { get; set; }

    public bool SendNonDeserializableResult { get; set; }

    public event EventHandler<VotifierEventArgs>? VoteReceived;

    private MockVotifierServer(TcpListener tcpListener)
    {
        _tcpListener = tcpListener;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _tcpListener.Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_tcpListener.Pending()) continue;
            var tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);

            await Task.Factory.StartNew(async () => await HandleConnection(tcpClient, cancellationToken), cancellationToken);
        }
    }

    private static string GetRandomBase64String()
    {
        // Write random challenge to header
        // For reference:
        // https://github.com/NuVotifier/go-votifier/blob/aa0f5221523bb12090dfcbd9ae4971a027edee9a/server.go#L68
        const int randomStringLength = 24;
        Span<char> randomChars = stackalloc char[randomStringLength];

        for (var i = 0; i < randomStringLength; i++)
        {
            var randomChar = (char)('A' + Random.Shared.Next(0, 24));
            randomChars[i] = randomChar;
        }

        return Convert.ToBase64String(MemoryMarshal.AsBytes(randomChars));
    }

    private async Task HandleConnection(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        // Protect CI runs from waiting a long time
        if (!Debugger.IsAttached)
        {
            tcpClient.ReceiveTimeout = 2000;
            tcpClient.SendTimeout = 2000;
        }

        await using var binaryWriter = new BinaryWriter(tcpClient.GetStream());
        using var binaryReader = new BinaryReader(tcpClient.GetStream());

        binaryWriter.Write(SendInvalidHeader
            // Write invalid header to verify a thrown VotifierException
            // Because tests should be deterministic this value is constant
            ? Encoding.UTF8.GetBytes("134985io\u1234jvf.mni9o23nm9$\\\"512\n")
            // Write random challenge to header
            // For reference:
            // https://github.com/NuVotifier/go-votifier/blob/aa0f5221523bb12090dfcbd9ae4971a027edee9a/server.go#L68
            : Encoding.UTF8.GetBytes($"VOTIFIER {(SendInvalidVersion ? "1" : "2")} {GetRandomBase64String()}\n"));

        binaryWriter.Flush();

        // Cancel if cancellation requested
        cancellationToken.ThrowIfCancellationRequested();

        // Verify magic number
        var magicNumber = BinaryPrimitives.ReadInt16BigEndian(binaryReader.ReadBytes(2));
        Assert.Equal(Voter.MagicNumber, magicNumber);

        var packetLength = BinaryPrimitives.ReadInt16BigEndian(binaryReader.ReadBytes(2));
        var packetJsonBytes = binaryReader.ReadBytes(packetLength);

        // Cancel if cancellation requested
        cancellationToken.ThrowIfCancellationRequested();

        var packetJson = Encoding.UTF8.GetString(packetJsonBytes);

        // Deserialize VotifierMessage from client and validate that it is not null
        var votifierMessage = JsonSerializer.Deserialize<VotifierMessage>(packetJson);
        Assert.NotNull(votifierMessage);

        // Validate payload signature
        using var shaHasher = HMAC.Create("HMACSHA256")!;
        shaHasher.Key = Encoding.UTF8.GetBytes(Token);
        var hash = shaHasher.ComputeHash(Encoding.UTF8.GetBytes(votifierMessage!.Payload!));
        Assert.Equal(votifierMessage.Signature, Convert.ToBase64String(hash));

        var votifierPayload = JsonSerializer.Deserialize<VotifierPayload>(votifierMessage.Payload!);
        var eventArgs = new VotifierEventArgs
        {
            Payload = votifierPayload!
        };
        InvokeVoteReceived(eventArgs);

        var resultJson = JsonSerializer.Serialize(eventArgs.Result);
        if (SendNonDeserializableResult)
        {
            // Manipulate `resultJson` so that no JsonDeserializer can still deserialize this.
            resultJson = resultJson.Replace('{', 'a');
            resultJson = resultJson.Replace('"', '`');
            resultJson = resultJson.Replace('}', '\u1234');
        }

        // Write resultJson and flush
        var resultBytes = Encoding.UTF8.GetBytes(resultJson);
        binaryWriter.Write(resultBytes);
        binaryWriter.Flush();

        // Terminate session
        tcpClient.Close();
    }

    internal static MockVotifierServer Create(int port)
    {
        // Verify that `port` is in the ushort bounds
        // Ports cannot be smaller than zero and not
        // greater than 65535
        Debug.Assert(port is > 0 and < ushort.MaxValue);

        var tcpListener = TcpListener.Create(port);
        return new MockVotifierServer(tcpListener);
    }

    private void InvokeVoteReceived(VotifierEventArgs e)
    {
        VoteReceived?.Invoke(this, e);
    }
}