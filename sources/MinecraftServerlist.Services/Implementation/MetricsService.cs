using MinecraftServerlist.Services.Abstractions;
using Prometheus;
using Prometheus.DotNetRuntime;
using System.Reflection;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class MetricsService : IMetricsService, IDisposable
{
    /*
    private static readonly Gauge JitCompilationTime = Metrics.CreateGauge("jit_compilation_time", "The amount of time the JIT Compiler has spent compiling methods");
    private static readonly Gauge JitCompiledMethods = Metrics.CreateGauge("jit_compiled_methods", "The amount of time the JIT Compiler has spent compiling methods.");
    private static readonly Gauge JitCompiledIlBytes = Metrics.CreateGauge("jit_compiled_il_bytes", "The amount of time the JIT Compiler has spent compiling methods.");
    */

    private static readonly Gauge RuntimeLoadedAssemblies =
        Metrics.CreateGauge("runtime_loaded_assemblies", "Total assembly count in AppDomain");

    private static readonly Histogram RuntimeUnhandledExceptions =
        Metrics.CreateHistogram("runtime_unhandled_exceptions", "Unhandled Exception Histogram", "type");

    private static readonly Histogram RuntimeAssemblyResolutionFail =
        Metrics.CreateHistogram("runtime_assembly_resolution_fail", "Assembly resolution failed", "requesting_assembly", "target_assembly");

    private static readonly Histogram RuntimeReflectionOnlyAssemblyResolutionFail =
        Metrics.CreateHistogram("runtime_reflection_assembly_resolution_fail", "Reflection-only assembly resolution failed", "requesting_assembly", "target_assembly");

    private static readonly Histogram RuntimeTypeResolutionFail =
        Metrics.CreateHistogram("runtime_type_resolution_fail", "Type resolution failed", "requesting_assembly", "target_type");

    private static readonly Histogram RuntimeResourceResolutionFail =
        Metrics.CreateHistogram("runtime_resource_resolution_fail", "Type resolution failed", "requesting_assembly", "target_type");

    private static readonly Counter SentMails = Metrics.CreateCounter("sent_mails", "The mail count");

    private readonly AppDomain _appDomain;
    private readonly IDisposable _runtimeStatsDisposable;

    public MetricsService()
    {
        _runtimeStatsDisposable = DotNetRuntimeStatsBuilder
            .Customize()
            .WithContentionStats()
            .WithJitStats()
            .WithThreadPoolStats()
            .WithGcStats()
            .WithExceptionStats()
            .StartCollecting();

        _appDomain = AppDomain.CurrentDomain;
        RuntimeLoadedAssemblies.Set(_appDomain.GetAssemblies().Length);

        _appDomain.AssemblyLoad += HandleAssemblyLoad;
        _appDomain.UnhandledException += HandleUnhandledException;

        _appDomain.AssemblyResolve += HandleAssemblyResolutionFail;
        _appDomain.ReflectionOnlyAssemblyResolve += HandleReflectionOnlyAssemblyResolutionFail;
        _appDomain.TypeResolve += HandleTypeResolutionFail;
        _appDomain.ResourceResolve += HandleResourceResolutionFail;
    }

    public void IncreaseMailCount()
    {
        SentMails.Inc();
    }

    private static Assembly? HandleAssemblyResolutionFail(object? sender, ResolveEventArgs args)
    {
        RuntimeAssemblyResolutionFail
            .WithLabels(args.RequestingAssembly?.FullName ?? "", args.Name)
            .Publish();

        return null;
    }

    private static Assembly? HandleReflectionOnlyAssemblyResolutionFail(object? sender, ResolveEventArgs args)
    {
        RuntimeReflectionOnlyAssemblyResolutionFail
            .WithLabels(args.RequestingAssembly?.FullName ?? "", args.Name)
            .Publish();

        return null;
    }

    private static Assembly? HandleTypeResolutionFail(object? sender, ResolveEventArgs args)
    {
        RuntimeTypeResolutionFail
            .WithLabels(args.RequestingAssembly?.FullName ?? "", args.Name)
            .Publish();

        return null;
    }

    private static Assembly? HandleResourceResolutionFail(object? sender, ResolveEventArgs args)
    {
        RuntimeResourceResolutionFail
            .WithLabels(args.RequestingAssembly?.FullName ?? "", args.Name)
            .Publish();

        return null;
    }

    private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exceptionType = $"[{e.ExceptionObject.GetType().Assembly.FullName}]::{e.ExceptionObject.GetType().FullName}";
        RuntimeUnhandledExceptions
            .WithLabels(exceptionType)
            .Publish();
    }

    private static void HandleAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        RuntimeLoadedAssemblies.Inc();
    }

    /*
    public void CollectJitCompilationStatistic()
    {
        var compilationTime = JitInfo.GetCompilationTime();
        var compiledMethodCount = JitInfo.GetCompiledMethodCount();
        var compiledIlBytes = JitInfo.GetCompiledILBytes();

        var compilationTimeTicks = compilationTime.TotalMilliseconds;

        JitCompilationTime
            .Set(compilationTimeTicks);
        JitCompiledMethods
            .Set(compiledMethodCount);
        JitCompiledIlBytes
            .Set(compiledIlBytes);
    }
    */

    public void Dispose()
    {
        _appDomain.AssemblyLoad -= HandleAssemblyLoad;
        _appDomain.UnhandledException -= HandleUnhandledException;
        _runtimeStatsDisposable.Dispose();
    }
}