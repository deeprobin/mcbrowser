using System.Collections.Immutable;
using System.Reflection;
using BenchmarkDotNet.Running;

Console.Title = "MinecraftServerlist Benchmark";

var assemblies = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetReferencedAssemblies();
var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

var benchmarkAssemblies =
    (from file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "MinecraftServerlist.*.Benchmarks.dll")
    select file).ToImmutableArray();

if (benchmarkAssemblies.Length < 1)
{
    Console.WriteLine("No benchmark assembly found");
}

foreach (var assemblyFile in benchmarkAssemblies)
{
    Assembly assembly;
    try
    {
        assembly = Assembly.LoadFile(assemblyFile);
    }
    catch (FileLoadException)
    {
        Console.Error.WriteLine($"Cannot load assembly: {assemblyFile} does exist but might be corrupted");
        continue;
    }
    catch (FileNotFoundException)
    {
        Console.Error.WriteLine($"Cannot load assembly: {assemblyFile} does not exist");
        continue;
    }
    catch (BadImageFormatException)
    {
        Console.Error.WriteLine($"Cannot load assembly: {assemblyFile} is not a .NET assembly");
        continue;
    }

    Console.WriteLine("------------------------------------");
    Console.WriteLine($"-- BENCHMARKING {assembly.GetName(false).Name} --");
    Console.WriteLine($"-- Benchmark Assembly: {assemblyFile} --");
    Console.WriteLine("------------------------------------");

    BenchmarkRunner.Run(assembly);
    Console.WriteLine("\n\n");
}