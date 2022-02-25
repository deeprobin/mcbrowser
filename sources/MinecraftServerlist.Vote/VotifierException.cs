using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MinecraftServerlist.Votifier;

public sealed class VotifierException : Exception
{
    internal VotifierException(string? message, Exception? nestedException = default) : base(message, nestedException)
    {
        // No logic here
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void Throw(string? message, Exception? nestedException = default)
    {
        throw new VotifierException(message, nestedException);
    }
}