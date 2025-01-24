// ReSharper disable CheckNamespace

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SmartAssembly.Attributes;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///     Exclude the type definition from pruning.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
public sealed class DoNotPruneAttribute : Attribute;