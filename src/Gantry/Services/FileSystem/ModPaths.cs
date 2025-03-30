using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem;

/// <summary>
///     Helper class for determining mod paths.
/// </summary>
public static class ModPaths
{
    /// <summary>
    ///     Gets the world unique identifier.
    /// </summary>
    public static string WorldGuid { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the root path for all VintageMods mod files.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string VintageModsRootPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path used for storing data files for a particular mod.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModDataRootPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path used for storing gantry data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store gantry files.</value>
    public static string ModDataGantryRootPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path used for storing gantry data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store gantry files.</value>
    public static string ModDataGantryGlobalPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path used for storing gantry data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store gantry files.</value>
    public static string ModDataGantryWorldPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path used for storing global data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModDataGlobalPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path used for storing per-world data files.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModDataWorldPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the path that the mod library is stored in.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModRootPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the main asset origin directory for the mod.
    /// </summary>
    /// <value>A path on the filesystem, used to store mod files.</value>
    public static string ModAssetsPath { get; internal set; } = string.Empty;

    /// <summary>
    ///     Creates a directory on the file-system.
    /// </summary>
    /// <param name="path">A path on the filesystem, used to store mod files.</param>
    /// <returns>Returns the absolute path to the directory that has been created.</returns>
    public static string CreateDirectory(string path)
    {
        var dir = new DirectoryInfo(path);
        if (dir.Exists) return dir.FullName;
        G.Logger.VerboseDebug($"Creating folder: {dir}");
        dir.Create();
        return dir.FullName;
    }

    internal static string GetScopedPath(string fileName, FileScope scope, bool gantryFile = false)
    {
        var directory = scope switch
        {
            FileScope.Global => gantryFile ? ModDataGantryGlobalPath : ModDataGlobalPath,
            FileScope.World => gantryFile ? ModDataGantryWorldPath : ModDataWorldPath,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };
        return Path.Combine(directory, fileName);
    }
}