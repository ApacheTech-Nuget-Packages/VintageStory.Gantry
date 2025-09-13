using Gantry.Tools.Common.Extensions;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Vintagestory.API.Config;

namespace Gantry.Tools.Common.ModLoader;

/// <summary>
///     Provides a custom assembly load context for mod packaging, enabling dynamic resolution and loading of assemblies
///     from multiple directories relevant to the Vintage Story modding environment. This context caches loaded assemblies
///     for efficiency and supports thread-safe management of search paths. It is designed to facilitate robust mod packaging
///     workflows by searching for both .dll and .exe assemblies, and can be extended for further custom resolution logic.
///     <para><b>Recommended:</b> Call <see cref="Unload"/> when done to release resources and unlock loaded files.</para>
/// </summary>
public class ModAssemblyLoadContext : AssemblyLoadContext, IDisposable
{
    private readonly ConcurrentBag<string> _resolvePaths;
    private readonly ConcurrentDictionary<string, Assembly> _assemblyCache = new();

    /// <summary>
    ///     Initialises a new instance of the <see cref="ModAssemblyLoadContext"/> class, setting up default search paths
    ///     including the current directory, Vintage Story installation, Mods, and Lib folders.
    /// </summary>
    public ModAssemblyLoadContext(string dependenciesDir) : base("Mods", isCollectible: true)
    {
        var runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
        var vintageStoryPath = Environment.GetEnvironmentVariable("VINTAGE_STORY")!;
        var initialPaths = new []
        {
            Environment.CurrentDirectory,
            Environment.SystemDirectory,
            vintageStoryPath,
            dependenciesDir,
            runtimePath
        };
        _resolvePaths = [.. initialPaths.Where(Directory.Exists)];

        LoadFromAssemblyPath(Path.Combine(vintageStoryPath, "VintagestoryAPI.dll"));
        LoadFromAssemblyPath(Path.Combine(vintageStoryPath, "VintagestoryLib.dll"));
        LoadFromAssemblyPath(Path.Combine(runtimePath, "System.Runtime.dll"));
        Resolving += (_, name) => Load(name);
    }

    /// <summary>
    ///     Attempts to resolve and load an assembly by name, using cached results if available, otherwise searching all
    ///     configured paths for a matching .dll or .exe file. Logs an error if the assembly cannot be found.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to resolve and load.</param>
    /// <returns>
    ///     The loaded <see cref="Assembly"/> if found; otherwise, <c>null</c>.
    /// </returns>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var name = assemblyName.Name!;
        if (_assemblyCache.TryGetValue(name, out var cached))
            return cached;
        var assembly = TryLoad(assemblyName);
        if (assembly is not null)
        {
            _assemblyCache[name] = assembly;
            return assembly;
        }
        return null;
    }

    /// <summary>
    ///     Attempts to load an assembly by searching all resolve paths for .dll or .exe files matching the assembly name.
    ///     Uses Directory.EnumerateFiles to recursively search for both extensions. This method is used internally by
    ///     the load context to resolve assemblies dynamically.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to load.</param>
    /// <returns>
    ///     The loaded <see cref="Assembly"/> if found; otherwise, <c>null</c>.
    /// </returns>
    private Assembly? TryLoad(AssemblyName assemblyName)
    {
        foreach (var path in _resolvePaths)
        {
            var dir = new DirectoryInfo(path);
            if (!dir.Exists) continue;

            var files = dir
                .EnumerateFiles($"{assemblyName.Name}.*", SearchOption.AllDirectories)
                .Where(f => f.Extension.In(".dll", ".exe"));

            var file = files.FirstOrDefault();
            if (file is not null && File.Exists(file.FullName))
                return LoadFromAssemblyPath(file.FullName);
        }
        return null;
    }

    /// <summary>
    ///     Loads an assembly from a specified file, adding its directory to the resolve paths if necessary. This method is
    ///     useful for loading assemblies that are not in the default search locations.
    /// </summary>
    /// <param name="assemblyFile">The file information for the assembly to load.</param>
    /// <returns>
    ///     The loaded <see cref="Assembly"/> if found; otherwise, <c>null</c>.
    /// </returns>
    private void AddDepsJsonDependencies(string assemblyDirectory, string assemblyName, HashSet<string>? visited = null)
    {
        visited ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var depsJsonPath = Path.Combine(assemblyDirectory, $"{assemblyName}.deps.json");
        if (!File.Exists(depsJsonPath) || !visited.Add(depsJsonPath)) return;

        using var stream = File.OpenRead(depsJsonPath);
        using var reader = new DependencyContextJsonReader();
        var context = reader.Read(stream);
        foreach (var lib in context.RuntimeLibraries)
        {
            foreach (var asset in lib.GetDefaultAssemblyNames(context))
            {
                // Try to find the assembly file in the current directory
                var name = asset.Name;
                var dllPath = Path.Combine(assemblyDirectory, name + ".dll");
                var exePath = Path.Combine(assemblyDirectory, name + ".exe");
                string? foundPath = null;
                if (File.Exists(dllPath)) foundPath = dllPath;
                else if (File.Exists(exePath)) foundPath = exePath;
                if (foundPath != null)
                {
                    var assetDir = Path.GetDirectoryName(foundPath)!;
                    if (!_resolvePaths.Contains(assetDir))
                    {
                        _resolvePaths.Add(assetDir);
                    }
                }
            }
            // Recursively process dependencies if their own deps.json exists
            var libDir = Path.Combine(assemblyDirectory, lib.Name);
            if (Directory.Exists(libDir))
            {
                AddDepsJsonDependencies(libDir, lib.Name, visited);
            }
        }
    }

    public Assembly? LoadAssemblyFromFileInfo(FileInfo assemblyFile)
    {
        var dir = Path.GetDirectoryName(assemblyFile.FullName)!;
        if (!_resolvePaths.Contains(dir)) _resolvePaths.Add(dir);
        // Scan for deps.json and add dependencies
        AddDepsJsonDependencies(dir, Path.GetFileNameWithoutExtension(assemblyFile.Name));
        var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyFile.FullName));
        return Load(assemblyName);
    }

    /// <summary>
    ///     Unloads the current assembly load context, releasing all cached assemblies and resolve paths.
    /// </summary>
    public void Dispose()
    {
        _assemblyCache.Clear();
        _resolvePaths.Clear();
        Unload();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.WaitForFullGCComplete();
        GC.SuppressFinalize(this);
    }
}