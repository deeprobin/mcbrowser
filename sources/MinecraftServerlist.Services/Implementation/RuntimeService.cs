using MinecraftServerlist.Common.Attributes;
using MinecraftServerlist.Services.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class RuntimeService : IRuntimeService
{
    private bool _intrinsicsPrepared;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void PrepareIntrinsics()
    {
        if (_intrinsicsPrepared)
        {
            // TODO: Log that the intrinsics are already prepared
            return;
        }

        var appDomain = AppDomain.CurrentDomain;
        var applicableAssemblies = from assembly in appDomain.GetAssemblies()
                                       // Only collect our assemblies
                                       // Third-party assemblies do not use the IntrinsicAttribute
                                   where assembly.FullName?.StartsWith(nameof(MinecraftServerlist)) == true
                                   select assembly;

        foreach (var assembly in applicableAssemblies)
        {
            PrepareIntrinsicsInAssembly(assembly);
        }

        _intrinsicsPrepared = true;
    }

    private static void PrepareIntrinsicsInAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            PrepareIntrinsicsInType(type);
        }
    }

    private static void PrepareIntrinsicsInType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type type)
    {
        if (type.IsEnum)
        {
            // Do not search Intrinsic methods in Enum-classes,
            // because we are currently not able to add Methods
            // to Enums (without using Custom IL)
            return;
        }

        if (type.IsInterface)
        {
            // Do not search Intrinsic methods in Interfaces
            return;
        }

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        foreach (var methodInfo in methods)
        {
            PreparePossibleIntrinsic(methodInfo);
        }
    }

    private static readonly Type IntrinsicAttributeType = typeof(IntrinsicAttribute);

    private static void PreparePossibleIntrinsic(MethodBase method)
    {
        var attribute = method.GetCustomAttribute(IntrinsicAttributeType);
        if (attribute is null) return;

        var intrinsicAttribute = (IntrinsicAttribute)attribute;

        var methodHandle = method.MethodHandle;

        switch (intrinsicAttribute.Level)
        {
            case IntrinsicAttributeLevel.Tier1:
                RuntimeHelpers.PrepareMethod(methodHandle);
                break;

            case IntrinsicAttributeLevel.Tier2:
                RuntimeHelpers.PrepareMethod(methodHandle);
                RuntimeHelpers.PrepareMethod(methodHandle);
                break;

            default:
                // TODO: Use on .NET 7 Target UnreachableException to enable some JIT optimizations
                throw new InvalidOperationException();
        }
    }
}