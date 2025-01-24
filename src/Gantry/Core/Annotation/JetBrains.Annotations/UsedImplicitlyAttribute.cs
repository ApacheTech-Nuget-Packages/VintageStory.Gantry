

// ReSharper disable CheckNamespace

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace JetBrains.Annotations;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///     Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
///     so this symbol will be ignored by usage-checking inspections. <br/>
///     You can use <see cref="ImplicitUseKindFlags"/> and <see cref="ImplicitUseTargetFlags"/>
///     to configure how this attribute is applied.
/// </summary>
/// <example><code>
/// [UsedImplicitly]
/// public class TypeConverter {}
/// 
/// public class SummaryData
/// {
///   [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
///   public SummaryData() {}
/// }
/// 
/// [UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.Default)]
/// public interface IService {}
/// </code></example>
[AttributeUsage(AttributeTargets.All)]
[Conditional("JETBRAINS_ANNOTATIONS")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class UsedImplicitlyAttribute : Attribute
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    public UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers) { }

    /// <summary>
    ///     Initialises a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    /// <param name="targetFlags">The target flags.</param>
    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    /// <summary>
    ///     Initialises a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    /// <param name="useKindFlags">The use kind flags.</param>
    /// <param name="targetFlags">The target flags.</param>
    public UsedImplicitlyAttribute(
        ImplicitUseKindFlags useKindFlags,
        ImplicitUseTargetFlags targetFlags = ImplicitUseTargetFlags.Default)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    /// <summary>
    ///     Gets the use kind flags.
    /// </summary>
    /// <value>
    ///     The use kind flags.
    /// </value>
    public ImplicitUseKindFlags UseKindFlags { get; }

    /// <summary>
    ///     Gets the target flags.
    /// </summary>
    /// <value>
    ///     The target flags.
    /// </value>
    public ImplicitUseTargetFlags TargetFlags { get; }
}