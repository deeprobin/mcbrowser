namespace MinecraftServerlist.Common.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate)]
public sealed class IntrinsicAttribute : Attribute
{
    public IntrinsicAttributeLevel Level { get; init; }

    public IntrinsicAttribute(IntrinsicAttributeLevel level = IntrinsicAttributeLevel.Tier1)
    {
        Level = level;
    }
}