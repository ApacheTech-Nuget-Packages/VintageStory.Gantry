// ReSharper disable CheckNamespace

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SmartAssembly.Attributes;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///     Exclude the type definition, as well as all type's members, from pruning.
/// </summary> 
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
public sealed class DoNotPruneTypeAttribute : Attribute;
