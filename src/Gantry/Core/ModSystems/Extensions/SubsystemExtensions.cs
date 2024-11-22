using System.Reflection;
using Gantry.Core.ModSystems.Abstractions;
using Vintagestory.API.Common;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Gantry.Core.ModSystems.Extensions;

/// <summary>
///     Provides extension methods for use with <see cref="GantrySubsystem"/> types.
/// </summary>
public static class SubsystemExtensions
{
    /// <summary>
    ///     Loads all concrete implementations of <see cref="GantrySubsystem" />
    ///     that are applicable for the specified <see cref="EnumAppSide" />.
    /// </summary>
    /// <param name="assemblies">The assemblies to search for subclasses of <see cref="GantrySubsystem" />.</param>
    /// <param name="appSide">The application side to filter subclasses by.</param>
    /// <returns>A collection of instantiated <see cref="GantrySubsystem" /> objects.</returns>
    public static IEnumerable<GantrySubsystem> LoadGantrySubsystems(this IEnumerable<Assembly> assemblies, EnumAppSide appSide) =>
        assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(GantrySubsystem)))
            .Where(type => type.IsApplicableForAppSide(appSide))
            .Select(type => Activator.CreateInstance(type) as GantrySubsystem)
            .Where(instance => instance is not null)
            .OrderBy(instance => instance.ExecuteOrder());

    /// <summary>
    ///     Determines if a type is applicable for the given <see cref="EnumAppSide" />.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="appSide">The application side.</param>
    /// <returns>True if the type is applicable; otherwise, false.</returns>
    private static bool IsApplicableForAppSide(this Type type, EnumAppSide appSide) =>
        appSide switch
        {
            EnumAppSide.Client => typeof(ClientSubsystem).IsAssignableFrom(type) ||
                                  typeof(UniversalSubsystem).IsAssignableFrom(type),
            EnumAppSide.Server => typeof(ServerSubsystem).IsAssignableFrom(type) ||
                                  typeof(UniversalSubsystem).IsAssignableFrom(type),
            EnumAppSide.Universal => typeof(UniversalSubsystem).IsAssignableFrom(type),
            _ => false
        };

    /// <summary>
    ///     Invokes the specified action for each subsystem in the collection.
    /// </summary>
    /// <param name="subsystems">The collection of subsystems to invoke the action on.</param>
    /// <param name="action">The action to invoke for each subsystem.</param>
    public static void InvokeForAll(this IEnumerable<GantrySubsystem> subsystems, Action<GantrySubsystem> action)
    {
        if (subsystems is null) return;
        foreach (var subsystem in subsystems) action(subsystem);
    }
}
