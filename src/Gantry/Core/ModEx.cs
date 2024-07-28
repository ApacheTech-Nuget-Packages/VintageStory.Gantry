using System.Reflection;
using ApacheTech.Common.Extensions.System;
using Gantry.Core.Diagnostics;
using Gantry.Core.Extensions.Api;
using Gantry.Services.FileSystem.Extensions;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

// ReSharper disable ConstantNullCoalescingCondition
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ConstantConditionalAccessQualifier

namespace Gantry.Core;

/// <summary>
///     Provides global access to mod information, and metadata.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ModEx
{
    /// <summary>
    ///     Gets the side designated within the mod information.
    /// </summary>
    /// <exception cref="GantryException">Cannot determine app-side before `ApiEx` is intialised.</exception>
    public static EnumAppSide ModAppSide { get; private set; }

    /// <summary>
    ///     The main assembly for the mod that initialised the Gantry MDK.
    /// </summary>
    public static Assembly ModAssembly { get; private set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to run Gantry in debug mode. This enables detailed logging, within the game log files.
    /// </summary>
    /// <value>
    ///   <c>true</c> if debug mode is enabled; otherwise, <c>false</c>.
    /// </value>
    public static bool DebugMode { get; set; }

    /// <summary>
    ///     The mod's metadata.
    /// </summary>
    public static ModInfo ModInfo { get; private set; }

    /// <summary>
    ///     Represents the current mod, as registered with the mod manager.
    /// </summary>
    public static Mod Mod { get; private set; }

    internal static void Initialise(Mod mod, Assembly modAssembly)
    {
        mod.Logger.GantryDebug($"[Gantry] Setting Mod details for: {mod.FileName}");
        Mod = Ensure.PopulatedWith(Mod, mod);

        mod.Logger.GantryDebug($"[Gantry] Setting Mod assembly as: {modAssembly.FullName}");
        ModAssembly = Ensure.PopulatedWith(ModAssembly, modAssembly);

        mod.Logger.GantryDebug($"[Gantry] Setting Mod Info for: {mod.Info.ModID}");
        ModInfo = Ensure.PopulatedWith(ModInfo, mod.Info);

        mod.Logger.GantryDebug("[Gantry] Creating Initial ModData Directory");
        CreateInitialDirectory();

        if (ModInfo is null)
            throw new GantryException(
                $"Could not find mod information for assembly: {ModAssembly.FullName}. " +
                "Are you missing a modinfo.json file, or `ModInfoAttribute` declaration?");

        mod.Logger.GantryDebug($"[Gantry] Setting ModAppSide as {ModInfo.Side}");
        ModAppSide = ModInfo.Side;
    }

    /// <summary>
    ///     Cleans up the mess I made of the previous attempt to fix Linux being a pain!
    /// </summary>
    private static string CreateInitialDirectory()
    {
        var baseDir = Path.Combine(GamePaths.DataPath, "ModData");

        var legacyFolderName = ModInfo.Authors[0].IfNullOrWhitespace("Gantry");
        var legacyFolderName2 = legacyFolderName.Replace(" ", "");
        var newFolderName = ModInfo.ToModID(legacyFolderName);

        var legacyDir = new DirectoryInfo(Path.Combine(baseDir, legacyFolderName));
        var legacyDir2 = new DirectoryInfo(Path.Combine(baseDir, legacyFolderName2));
        var newDir = new DirectoryInfo(Path.Combine(baseDir, newFolderName));

        // Fix my failed fix.
        if (legacyDir2.Exists) legacyDir2.Rename(newFolderName);

        // Fix the original issue.
        if (legacyDir.Exists) legacyDir.Rename(newFolderName);

        // New players caught in the crossfire.
        if (!newDir.Exists) newDir.Create();

        return newDir.FullName;
    }

    /// <summary>
    ///     Determines whether a given mod is installed, and enabled, on the current app-side.
    /// </summary>
    /// <param name="modId">The mod identifier.</param>
    /// <param name="api">The api.</param>
    /// <returns><c>true</c> if the mod is enabled; otherwise, <c>false</c>.</returns>
    public static bool IsModEnabled(string modId, ICoreAPI api = null)
    {
        api ??= ApiEx.Current;
        var modEnabled = api.ModLoader.IsModEnabled(modId);
        return modEnabled;
    }

    /// <summary>
    ///     Determines whether the current code block is running on the main thread. See remarks.
    /// </summary>
    /// <remarks>
    ///     Within a Single-Player game, the server will never run on the main application thread.
    ///     Single-Player servers are run as a background thread, within the client application.
    /// </remarks>
    /// <returns><c>true</c> if the code is currently running on the main application thread; otherwise <c>false</c>.</returns>
    public static bool IsCurrentlyOnMainThread()
    {
        var thread = Thread.CurrentThread;
        var mainThread = ApiEx.MainThread;
        return mainThread is not null
            ? mainThread.ManagedThreadId == thread.ManagedThreadId
            : thread.GetApartmentState() == ApartmentState.STA
              && !thread.IsBackground
              && !thread.IsThreadPoolThread
              && thread.IsAlive;
    }
}