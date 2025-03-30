using System.Reflection;
using Gantry.Core.ModSystems.Abstractions;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Gantry.Core.ModSystems.Extensions;

/// <summary>
///     Provides extension methods for use with <see cref="GantrySubsystem"/> types.
/// </summary>
internal static class SubsystemExtensions
{
    /// <summary>
    ///     Loads all types that derive from <see cref="GantrySubsystem" />
    ///     that are applicable for the specified <see cref="EnumAppSide" />.
    /// </summary>
    /// <param name="assemblies">The assemblies to search for subclasses of <see cref="GantrySubsystem" />.</param>
    /// <returns>A collection of instantiated <see cref="GantrySubsystem" /> objects.</returns>
    internal static IEnumerable<GantrySubsystem> LoadGantrySubsystems(this IEnumerable<Assembly> assemblies) =>
        assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(GantrySubsystem)))
            .Select(p => Activator.CreateInstance(p) as GantrySubsystem)
            .Where(p => p is not null && p.Enabled)!;

    internal static IEnumerable<GantrySubsystem> For(this IEnumerable<GantrySubsystem> subsystems, EnumAppSide side) 
        => subsystems.Where(p => p.ShouldLoad(side)).OrderBy(p => p.ExecuteOrder());

    /// <summary>
    ///     Determines if a type is applicable for the given <see cref="EnumAppSide" />.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="appSide">The application side.</param>
    /// <returns>True if the type is applicable; otherwise, false.</returns>
    internal static bool IsApplicableForAppSide(this Type type, EnumAppSide appSide) =>
        appSide switch
        {
            EnumAppSide.Client => typeof(ClientSubsystem).IsAssignableFrom(type) ||
                                  typeof(UniversalSubsystem).IsAssignableFrom(type),
            EnumAppSide.Server => typeof(ServerSubsystem).IsAssignableFrom(type) ||
                                  typeof(UniversalSubsystem).IsAssignableFrom(type),
            EnumAppSide.Universal => typeof(UniversalSubsystem).IsAssignableFrom(type),
            _ => false
        };
}