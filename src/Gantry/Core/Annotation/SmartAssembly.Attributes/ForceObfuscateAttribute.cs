// ReSharper disable CheckNamespace

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SmartAssembly.Attributes;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///     Force element to be obfuscated, even if it was excluded by safety mechanisms.
///     Takes precedence over SmartAssembly.Attributes.DoNotObfuscateAttribute!
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface)]
public sealed class ForceObfuscateAttribute : Attribute
{  
    /// <summary>
    ///     Force element to be obfuscated, even if it was excluded by safety mechanisms.
    ///     Takes precedence over SmartAssembly.Attributes.DoNotObfuscateAttribute!
    /// </summary>
    /// <param name="useHashAsName">If true, uses MD5 hash of a method name prefixed with "_". Otherwise, uses default Name Mangling setting.</param>
    public ForceObfuscateAttribute([UsedImplicitly] bool useHashAsName = false) { }
}