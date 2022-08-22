using System;
using System.IO;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Services.FileSystem.Enums;
using Vintagestory.API.Config;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.FileSystem
{
    /// <summary>
    ///     Helper class for determining mod paths.
    /// </summary>
    public static class ModPaths
    {
        /// <summary>
        /// 	Initialises static members of the <see cref="ModPaths" /> class.
        /// </summary>
        public static void Initialise(string rootDirectoryName)
        {
            ModDataRootPath = CreateDirectory(Path.Combine(VintageModsRootPath,
                rootDirectoryName.IfNullOrWhitespace(ModEx.ModInfo.ModID ?? Guid.NewGuid().ToString())));
            ModDataGlobalPath = CreateDirectory(Path.Combine(ModDataRootPath, "Global"));
            ModDataWorldPath = CreateDirectory(Path.Combine(ModDataRootPath, "World", ApiEx.Current.World.SavegameIdentifier)); 
            ModRootPath = Path.GetDirectoryName(ModEx.ModAssembly.Location)!;
            ModAssetsPath = Path.Combine(ModRootPath, "assets");
        }

        /// <summary>
        ///     Gets the root path for all VintageMods mod files.
        /// </summary>
        /// <value>A path on the filesystem, used to store mod files.</value>
        public static string VintageModsRootPath { get; } = CreateDirectory(
            Path.Combine(GamePaths.DataPath, "ModData", ModEx.ModInfo.Authors[0].IfNullOrWhitespace("Gantry")));

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
        public static string ModDataWorldPath { get; internal set; }

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

        internal static string GetScopedPath(FileScope scope, string fileName)
        {
            var directory = scope switch
            {
                FileScope.Global => ModDataGlobalPath,
                FileScope.World => ModDataWorldPath,
                FileScope.Local => ModRootPath,
                _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
            };

            if (scope is not FileScope.Local) return Path.Combine(directory, fileName);
            if (File.Exists(directory)) return Path.Combine(directory, fileName);

            var files = Directory.GetFiles(ModRootPath, fileName, SearchOption.AllDirectories);
            return files.Length switch
            {
                1 => files[0],
                < 1 => throw new FileNotFoundException($"Local file, `{fileName}`, does not exist within the mod folder."),
                > 1 => throw new FileLoadException($"Local file, `{fileName}`, is duplicated within the mod folder.")
            };
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
        }
    }
}