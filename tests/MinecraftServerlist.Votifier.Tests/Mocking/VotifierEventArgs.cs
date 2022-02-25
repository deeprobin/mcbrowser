namespace MinecraftServerlist.Votifier.Tests.Mocking;

internal sealed class VotifierEventArgs : EventArgs
{
    internal VotifierPayload Payload { get; init; } = default!;

    internal VotifierResult? Result { get; set; } = new();
}