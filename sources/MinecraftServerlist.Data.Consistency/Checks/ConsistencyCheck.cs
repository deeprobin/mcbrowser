using JetBrains.Annotations;
using MinecraftServerlist.Data.Consistency.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinecraftServerlist.Data.Consistency.Checks;

public abstract class ConsistencyCheck : INotifyPropertyChanged
{
    private uint _progressValue;
    private uint _progressMax;

    public uint ProgressValue
    {
        get => _progressValue;
        protected set
        {
            _progressValue = value;
            InvokePropertyChanged();
        }
    }

    public uint ProgressMax
    {
        get => _progressMax;
        protected set
        {
            _progressMax = value;
            InvokePropertyChanged();
        }
    }

    protected Task ReportDiagnosticAsync(ConsistencyDiagnostic diagnostic,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    protected abstract Task Run(CancellationToken cancellationToken = default);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void InvokePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}