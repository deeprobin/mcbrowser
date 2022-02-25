using System.Buffers.Binary;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MinecraftServerlist.Votifier;

public static class Voter
{
    internal const string ServiceName = "mcbrowser.net";
    internal const short MagicNumber = 0x733a;

    private const string VotifierProtocolIdentifier = "VOTIFIER";
    private const string VotifierOkConstant = "ok";
    private const string VotifierErrorConstant = "error";

    // See logic in https://github.com/NuVotifier/votifier2-php/blob/master/src/Server.php
    public static async Task SubmitVote(string host, int port = 8192, string minecraftUsername = "", string token = "", long timestamp = 0, CancellationToken cancellationToken = default)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(host, port, cancellationToken);

        await using var stream = tcpClient.GetStream();
        var reader = new StreamReader(stream);

        var header = await reader.ReadLineAsync();

        // Verify v2 protocol
        var headerParts = header!.Split(' ');
        if (headerParts.Length == 3)
        {
            if (headerParts[1] != "2")
            {
                VotifierException.Throw("Unsupported votifier version");
            }
        }
        else if (headerParts[0] != VotifierProtocolIdentifier)
        {
            VotifierException.Throw("Cannot parse VOTIFIER-header");
        }
        else
        {
            VotifierException.Throw("Corrupted header");
        }

        var challenge = headerParts[2];

        var payload = new VotifierPayload
        {
            Address = host,
            ServiceName = ServiceName,
            Challenge = challenge,
            Timestamp = timestamp,
            Username = minecraftUsername
        };

        var payloadJson = JsonSerializer.Serialize(payload);

        using var shaHasher = HMAC.Create("HMACSHA256")!;
        shaHasher.Key = Encoding.UTF8.GetBytes(token);

        var hashedSignature = shaHasher.ComputeHash(Encoding.UTF8.GetBytes(payloadJson));
        var signature = Convert.ToBase64String(hashedSignature);

        var message = new VotifierMessage
        {
            Signature = signature,
            Payload = payloadJson
        };

        var messageJson = JsonSerializer.Serialize(message);

        var magicNumberBytes = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(magicNumberBytes, MagicNumber);

        await stream.WriteAsync(magicNumberBytes, cancellationToken);

        var strLenBytes = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(strLenBytes, (short)messageJson.Length);

        await stream.WriteAsync(strLenBytes, cancellationToken);
        await stream.WriteAsync(Encoding.UTF8.GetBytes(messageJson), cancellationToken);

        await stream.FlushAsync(cancellationToken);

        var resultJson = await reader.ReadToEndAsync();
        VotifierResult result = default!;
        try
        {
            var nullableResult = JsonSerializer.Deserialize<VotifierResult>(resultJson);

            if (nullableResult is null)
            {
                VotifierException.Throw("Cannot deserialize result: VotifierResult is null");
            }

            result = nullableResult;
        }
        catch (JsonException ex)
        {
            VotifierException.Throw($"Cannot deserialize result: ${ex.Message}", ex);
        }

        var resultStatus = result.Status;
        if (resultStatus != VotifierOkConstant)
        {
            if (resultStatus == VotifierErrorConstant)
            {
                VotifierException.Throw($"Erroneous VotifierResult: ${result.Error}");
            }

            VotifierException.Throw($"Unspecified resultStatus {resultStatus}");
        }
    }
}