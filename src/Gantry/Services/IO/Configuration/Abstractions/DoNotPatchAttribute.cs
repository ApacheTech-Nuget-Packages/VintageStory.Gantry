using Gantry.Core.Annotation;

namespace Gantry.Services.IO.Configuration.Abstractions;

/// <summary>
///     Marks a property to be excluded from automated configuration patching.
///     When applied, the configuration patching service will skip the attributed
///     property during any merge or patch operations.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DoNotPatchAttribute(EnumAppSide side = EnumAppSide.Universal) : SidedAttribute(side);