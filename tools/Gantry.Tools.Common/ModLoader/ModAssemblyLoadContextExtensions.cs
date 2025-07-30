using System.Reflection;

namespace Gantry.Tools.Common.ModLoader;

/// <summary>
///     Provides extension methods for loading assemblies with a custom load context.
/// </summary>
public static class ModAssemblyLoadContextExtensions
{
    /// <summary>
    ///     Loads an assembly from the specified file using a custom AssemblyLoadContext and applies the provided function to it.
    ///     Disposes the context after use to ensure proper unloading and resource cleanup.
    /// </summary>
    /// <typeparam name="T">The return type of the function applied to the loaded assembly.</typeparam>
    /// <param name="assemblyFile">The assembly file to load.</param>
    /// <param name="dependenciesDir">The directory containing dependencies for the load context.</param>
    /// <param name="f">A function to apply to the loaded assembly.</param>
    /// <returns>The result of the function applied to the loaded assembly.</returns>
    public static T? WithAssemblyContext<T>(this FileInfo assemblyFile, string dependenciesDir, Func<Assembly, T> f)
    {
        using var context = new ModAssemblyLoadContext(dependenciesDir);
        var assembly = context.LoadFromAssemblyPath(assemblyFile.FullName);
        return f(assembly);
    }
}