// ReSharper disable CheckNamespace

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace JetBrains.Annotations;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///     Specifies the details of implicitly used symbol when it is marked
///     with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
/// </summary>
[Flags]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public enum ImplicitUseKindFlags
{
    /// <summary>
    ///    By default: Access | Assign | InstantiatedWithFixedConstructorSignature
    /// </summary>
    Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

    /// <summary>
    ///     Only entity marked with attribute considered used.
    /// </summary>
    Access = 1,

    /// <summary>
    ///     Indicates implicit assignment to a member.
    /// </summary>
    Assign = 2,

    /// <summary>
    ///     Indicates implicit instantiation of a type with fixed constructor signature.
    ///     That means any unused constructor parameters won't be reported as such.
    /// </summary>
    InstantiatedWithFixedConstructorSignature = 4,

    /// <summary>
    ///     Indicates implicit instantiation of a type.
    /// </summary>
    InstantiatedNoFixedConstructorSignature = 8,
}