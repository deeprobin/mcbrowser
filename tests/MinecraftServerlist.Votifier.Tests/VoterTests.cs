using MinecraftServerlist.Votifier.Tests.Mocking;

namespace MinecraftServerlist.Votifier.Tests;

public sealed class VoterTests
{
    private static int GetNextPort() => Random.Shared.Next(8190, 8600);

    [Theory]
    [InlineData("oa_kad3afmAKJ!AWFMi14oi99013")]
    [InlineData("oä#\u123456\\abc-def-hij-klm")]
    public async Task TestOk(string token)
    {
        var port = GetNextPort();

        var votifierServer = MockVotifierServer.Create(port);
        votifierServer.Token = token;

        var cancellationTokenSource = new CancellationTokenSource();

        votifierServer.VoteReceived += delegate (object? _, VotifierEventArgs args) { args.Result!.Status = "ok"; };

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        try
        {
            await Voter.SubmitVote("localhost", port, "username", token,
                cancellationToken: cancellationTokenSource.Token);
        }
        finally
        {
            cancellationTokenSource.Cancel();
        }
    }

    [Theory]
    [InlineData("oa_kad3afmAKJ!AWFMi14oi99013")]
    [InlineData("oä#\u123456\\abc-def-hij-klm")]
    public async Task TestInvalidToken(string token)
    {
        var port = GetNextPort();

        var votifierServer = MockVotifierServer.Create(port);
        votifierServer.Token = "invalid";

        var cancellationTokenSource = new CancellationTokenSource();

        votifierServer.VoteReceived += delegate (object? _, VotifierEventArgs args) { args.Result!.Status = "ok"; };

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        try
        {
            await Assert.ThrowsAsync<VotifierException>(async () => await Voter.SubmitVote("localhost", port,
                "username",
                token,
                cancellationToken: cancellationTokenSource.Token));
        }
        finally
        {
            cancellationTokenSource.Cancel();
        }
    }

    [Theory]
    [InlineData("oa_kad3afmAKJ!AWFMi14oi99013", "username1")]
    [InlineData("oä#\u123456\\abc-def-hij-klm", "username2")]
    public async Task TestPayload(string token, string username)
    {
        var port = GetNextPort();

        var votifierServer = MockVotifierServer.Create(port);
        votifierServer.Token = token;

        var cancellationTokenSource = new CancellationTokenSource();

        VotifierPayload? payload = default;
        var resetEvent = new ManualResetEventSlim();
        votifierServer.VoteReceived += delegate (object? _, VotifierEventArgs args)
        {
            args.Result!.Status = "ok";
            payload = args.Payload;
            resetEvent.Set();
        };

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        try
        {
            await Voter.SubmitVote("localhost", port, username, token,
                cancellationToken: cancellationTokenSource.Token);
        }
        finally
        {
            cancellationTokenSource.Cancel();
        }

        resetEvent.Wait(CancellationToken.None);

        Assert.NotNull(payload);
        Assert.Equal(username, payload!.Username);
        Assert.Equal("localhost", payload.Address);
        Assert.Equal(Voter.ServiceName, payload.ServiceName);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("error")]
    [InlineData("")]
    [InlineData("\uf080\uf081\uf082")]
    public async Task TestError(string errorMessage)
    {
        var port = GetNextPort();
        var votifierServer = MockVotifierServer.Create(port);
        var cancellationTokenSource = new CancellationTokenSource();

        votifierServer.VoteReceived += delegate (object? _, VotifierEventArgs args)
        {
            args.Result!.Status = "error";
            args.Result.Error = errorMessage;
        };

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        try
        {
            var exception = await Assert.ThrowsAsync<VotifierException>(async () =>
                await Voter.SubmitVote("localhost", port, "username",
                    cancellationToken: cancellationTokenSource.Token));
            Assert.Contains(errorMessage, exception.Message);
        }
        finally
        {
            cancellationTokenSource.Cancel();
        }
    }

    [Fact]
    public async Task TestInvalidHeader()
    {
        var port = GetNextPort();
        var votifierServer = MockVotifierServer.Create(port);

        votifierServer.SendInvalidHeader = true;

        var cancellationTokenSource = new CancellationTokenSource();

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        await Assert.ThrowsAsync<VotifierException>(async () => await Voter.SubmitVote("localhost", port,
            "username",
            cancellationToken: cancellationTokenSource.Token));
    }

    [Fact]
    public async Task TestInvalidVersion()
    {
        var port = GetNextPort();
        var votifierServer = MockVotifierServer.Create(port);

        votifierServer.SendInvalidVersion = true;

        var cancellationTokenSource = new CancellationTokenSource();

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        await Assert.ThrowsAsync<VotifierException>(async () => await Voter.SubmitVote("localhost", port,
            "username",
            cancellationToken: cancellationTokenSource.Token));
    }

    [Fact]
    public async Task TestInvalidConstantResult()
    {
        var port = GetNextPort();
        var votifierServer = MockVotifierServer.Create(port);

        votifierServer.VoteReceived += delegate (object? _, VotifierEventArgs args) { args.Result!.Status = "xyz"; };

        var cancellationTokenSource = new CancellationTokenSource();

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        await Assert.ThrowsAsync<VotifierException>(async () => await Voter.SubmitVote("localhost", port,
            "username",
            cancellationToken: cancellationTokenSource.Token));
    }

    [Fact]
    public async Task TestNullResult()
    {
        var port = GetNextPort();
        var votifierServer = MockVotifierServer.Create(port);

        votifierServer.VoteReceived += delegate (object? _, VotifierEventArgs args) { args.Result = null; };

        var cancellationTokenSource = new CancellationTokenSource();

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        await Assert.ThrowsAsync<VotifierException>(async () => await Voter.SubmitVote("localhost", port,
            "username",
            cancellationToken: cancellationTokenSource.Token));
    }

    [Fact]
    public async Task TestWrongResult()
    {
        var port = GetNextPort();
        var votifierServer = MockVotifierServer.Create(port);

        votifierServer.SendNonDeserializableResult = true;

        var cancellationTokenSource = new CancellationTokenSource();

        _ = Task.Factory.StartNew(() => votifierServer.Run(cancellationTokenSource.Token),
            cancellationTokenSource.Token);
        await Assert.ThrowsAsync<VotifierException>(async () => await Voter.SubmitVote("localhost", port,
            "username",
            cancellationToken: cancellationTokenSource.Token));
    }
}