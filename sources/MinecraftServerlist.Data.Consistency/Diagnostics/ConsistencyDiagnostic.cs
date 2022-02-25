using MinecraftServerlist.Data.Consistency.Fixes;

namespace MinecraftServerlist.Data.Consistency.Diagnostics;

public sealed class ConsistencyDiagnostic
{
    public ConsistencyDiagnostic(string message, DiagnosticSeverity severity = DiagnosticSeverity.Perfectionism, IConsistencyFix? fix = default)
    {
        Message = message;
        Severity = severity;
        Fix = default;
    }

    public string Message { get; }

    public DiagnosticSeverity Severity { get; }

    public IConsistencyFix? Fix { get; }
}