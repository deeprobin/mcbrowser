using BenchmarkDotNet.Attributes;

namespace MinecraftServerlist.Votifier.Benchmarks;

public class ThrowVotifierExceptionBenchmark
{
    private readonly string _message;

    public ThrowVotifierExceptionBenchmark()
    {
        // Constant for better comparison
        const int length = 32;

        _message = "";
        for (var i = 0; i < length; i++)
        {
            _message += Random.Shared.Next('a', 'z');
        }
    }

    [Benchmark(Description = $"Throw {nameof(VotifierException)} (using `throw`)", Baseline = true)]
    public void BenchmarkVotifierExceptionThrowKeyword()
    {
        try
        {
            throw new VotifierException(_message);
        }
        catch (VotifierException)
        {

        }
    }

    [Benchmark(Description = $"Throw {nameof(VotifierException)} (using `{nameof(VotifierException)}.{nameof(VotifierException.Throw)}`)")]
    public void BenchmarkVotifierExceptionThrowMethod()
    {
        try
        {
            VotifierException.Throw(_message);
        }
        catch (VotifierException)
        {

        }
    }
}