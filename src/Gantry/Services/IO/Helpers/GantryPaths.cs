using Gantry.Core.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Helpers;

/// <summary>
///     Provides strongly-typed paths for Gantry mod configuration and data storage.
///     Paths are scoped to the current mod and world context, ensuring correct isolation between mods and worlds.
/// </summary>
public class GantryPaths(ICoreGantryAPI gantry)
{
    private readonly ICoreGantryAPI _gantry = gantry;

    /// <summary>
    ///     Retrieves the appropriate directory for a given mod file type and scope.
    /// </summary>
    /// <param name="type">The type of mod file.</param>
    /// <param name="scope">The scope of the mod file, which can be World, Global, or Gantry.</param>
    /// <returns>The directory where the mod file should be stored.</returns>
    /// <exception cref="UnreachableException"></exception>
    public DirectoryInfo For(ModFileType type, ModFileScope scope)
    {
        return type switch
        {
            ModFileType.Assets => ModAssets,
            ModFileType.Settings => scope switch
            {
                ModFileScope.World => WorldSettings,
                ModFileScope.Global => GlobalSettings,
                ModFileScope.Gantry => GantrySettings,
                _ => throw new UnreachableException()
            },
            ModFileType.Data => scope switch
            {
                ModFileScope.World => WorldData,
                ModFileScope.Global => GlobalData,
                ModFileScope.Gantry => GantryData,
                _ => throw new UnreachableException()
            },
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     The unique identifier for the current world, used to scope settings and data directories.
    /// </summary>
    public string WorldGuid 
        => _gantry.Uapi.World.SavegameIdentifier;

    /// <summary>
    ///     The root directory for the current mod, where the mod assembly is located.
    /// </summary>
    public DirectoryInfo ModRootPath 
        => new(_gantry.ModAssembly.Location!);

    /// <summary>
    ///     The root path for the assets of the current mod.
    /// </summary>
    public DirectoryInfo ModAssets 
        => new(Path.Combine(ModRootPath.FullName, "assets"));

    /// <summary>
    ///     Path to the world-specific settings directory for the current mod.
    /// </summary>
    public DirectoryInfo WorldSettings 
        => Directory.CreateDirectory(Path.Combine(GamePaths.ModConfig, _gantry.Mod.Info.ModID, "World", WorldGuid));

    /// <summary>
    ///     Path to the global settings directory for the current mod.
    /// </summary>
    public DirectoryInfo GlobalSettings 
        => Directory.CreateDirectory(Path.Combine(GamePaths.ModConfig, _gantry.Mod.Info.ModID, "Global"));

    /// <summary>
    ///     Path to the Gantry global settings directory, shared across all mods using Gantry.
    /// </summary>
    public DirectoryInfo GantrySettings 
        => Directory.CreateDirectory(Path.Combine(GamePaths.ModConfig, "Gantry"));

    /// <summary>
    ///     Path to the world-specific data directory for the current mod.
    /// </summary>
    public DirectoryInfo WorldData 
        => Directory.CreateDirectory(Path.Combine(GamePaths.DataPath, "ModData", _gantry.Mod.Info.ModID, "World", WorldGuid));

    /// <summary>
    ///     Path to the global data directory for the current mod.
    /// </summary>
    public DirectoryInfo GlobalData 
        => Directory.CreateDirectory(Path.Combine(GamePaths.DataPath, "ModData", _gantry.Mod.Info.ModID, "Global"));

    /// <summary>
    ///     Path to the Gantry global data directory, shared across all mods using Gantry.
    /// </summary>
    public DirectoryInfo GantryData 
        => Directory.CreateDirectory(Path.Combine(GamePaths.DataPath, "ModData", "Gantry"));
}