using System.Reflection;
using System.Runtime.Loader;

namespace Gantry.Core;

/// <summary>
///     Direct access to the game's vanilla assemblies.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class GameAssemblies
{
    /// <summary>
    ///     VSEssentials.dll
    /// </summary>
    public static Assembly VSEssentials => GetAssembly("VSEssentials");

    /// <summary>
    ///     VSSurvivalMod.dll
    /// </summary>
    public static Assembly VSSurvivalMod => GetAssembly("VSSurvivalMod");

    /// <summary>
    ///     VSCreativeMod.dll
    /// </summary>
    public static Assembly VSCreativeMod => GetAssembly("VSCreativeMod");

    /// <summary>
    ///     VintagestoryAPI.dll
    /// </summary>
    public static Assembly VintagestoryAPI => GetAssembly("VintagestoryAPI");

    /// <summary>
    ///     VintagestoryLib.dll
    /// </summary>
    public static Assembly VintagestoryLib => GetAssembly("VintagestoryLib");

    /// <summary>
    ///     Vintagestory.exe
    /// </summary>
    public static Assembly VintagestoryExe => Assembly.GetEntryAssembly();

    /// <summary>
    ///     Retrieves a list of all the assemblies collated within the <see cref="GameAssemblies"/> class. 
    /// </summary>
    public static IEnumerable<Assembly> All
    {
        get
        {
            return typeof(GameAssemblies)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Assembly))
                .Select(prop => (Assembly)prop.GetValue(null))
                .ToList();
        }
    }

    /// <summary>
    ///     Scans for a specific type within one of the game's vanilla assemblies. Includes internal classes, and nested
    ///     private classes. It can then be instantiated via Harmony.
    /// </summary>
    /// <param name="assembly">The assembly to scan within.</param>
    /// <param name="typeName">The name of the type to scan for.</param>
    /// <returns>The Type definition of the object being scanned for.</returns>
    public static Type FindType(this Assembly assembly, string typeName)
    {
        return AccessTools
            .GetTypesFromAssembly(assembly)
            .FirstOrDefault(t => t.Name == typeName);
    }

    /// <summary>
    ///     Scans for a specific type within the game's vanilla assemblies. Includes internal classes, and nested private
    ///     classes. It can then be instantiated via Harmony.
    /// </summary>
    /// <param name="typeName">The name of the type to scan for.</param>
    /// <returns>The Type definition of the object being scanned for.</returns>
    public static Type FindType(string typeName)
    {
        return All
            .Select(assembly => assembly.FindType(typeName))
            .FirstOrDefault();
    }

    private static Assembly GetAssembly(string name)
    {
        return GetLoadedAssemblies().FirstOrDefault(a => a.GetName().Name == name);
    }

    private static IEnumerable<Assembly> GetLoadedAssemblies()
    {
        return AssemblyLoadContext.Default.Assemblies;
    }
}