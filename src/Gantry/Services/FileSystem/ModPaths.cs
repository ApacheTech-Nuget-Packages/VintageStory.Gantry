using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.Diagnostics;
using Gantry.Services.FileSystem.Enums;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.FileSystem;

/// <summary>
///     Helper class for determining mod paths.
/// </summary>
public static class ModPaths
{
    /// <summary>
    /// 	Initialises static members of the <see cref="ModPaths" /> class.
    /// </summary>
    public static void Initialise(string rootDirectoryName, string worldIdentifier)
    {
        WorldGuid = Ensure.PopulatedWith(WorldGuid, worldIdentifier);
        VintageModsRootPath = Path.Combine(Path.Combine(GamePaths.DataPath, "ModData"),
            ModInfo.ToModID(ModEx.ModInfo.Authors[0].IfNullOrWhitespace("Gantry")));
        ModDataRootPath = CreateDirectory(Path.Combine(VintageModsRootPath,
            rootDirectoryName.IfNullOrWhitespace(ModEx.ModInfo.ModID ?? Guid.NewGuid().ToString())));
        ModDataGlobalPath = CreateDirectory(Path.Combine(ModDataRootPath, "Global"));
        ModDataWorldPath = CreateDirectory(Path.Combine(ModDataRootPath, worldIdentifier)); 
        ModRootPath = Path.GetDirectoryName(ModEx.ModAssembly.Location)!;
        ModAssetsPath = Path.Combine(ModRootPath, "assets");
    }

    /// <summary>
    ///     Gets the world unique identifier.
    /// </summary>
    public static string WorldGuid { get; private set; }

    /// <summary>
    ///     Gets the root path for all VintageMods mod files.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string VintageModsRootPath { get; private set; }

    /// <summary>
    ///     Gets the path used for storing data files for a particular mod.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModDataRootPath { get; private set; }

    /// <summary>
    ///     Gets the path used for storing global data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModDataGlobalPath { get; private set; }

    /// <summary>
    ///     Gets the path used for storing per-world data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModDataWorldPath { get; private set; }

    /// <summary>
    ///     Gets the path that the mod library is stored in.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModRootPath { get; private set; }

    /// <summary>
    ///     Gets the main asset origin directory for the mod.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModAssetsPath { get; private set; }

    /// <summary>
    ///     Creates a directory on the file-system.
    /// </summary>
    /// <param name="path">A path on the filesystem, used to store mod files.</param>
    /// <returns>Returns the absolute path to the directory that has been created.</returns>
    public static string CreateDirectory(string path)
    {
        var dir = new DirectoryInfo(path);
        if (dir.Exists) return dir.FullName;
        ApiEx.Current?.Logger.VerboseDebug($"[VintageMods] Creating folder: {dir}");
        dir.Create();
        return dir.FullName;
    }

    internal static string GetScopedPath(string fileName, FileScope scope)
    {
        var directory = scope switch
        {
            FileScope.Global => ModDataGlobalPath,
            FileScope.World => ModDataWorldPath,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };
        return Path.Combine(directory, fileName);
    }
        
    /// <summary>
    ///     DEV NOTE: Stops world settings files from being transferred between worlds.
    /// </summary>
    internal static void Dispose()
    {
        ModDataRootPath = null;
        ModDataGlobalPath = null;
        ModDataWorldPath = null;
        ModRootPath = null;
        ModAssetsPath = null;
        WorldGuid = null;
    }
}