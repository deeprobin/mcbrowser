namespace MinecraftServerlist.Data.Consistency.Fixes;

public interface IConsistencyFix
{
    public Task ApplyFixAsync(CancellationToken cancellationToken = default);
}