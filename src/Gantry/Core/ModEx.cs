using System.Reflection;
using Gantry.Core.Diagnostics;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core;

/// <summary>
///     Provides global access to mod information, and metadata.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ModEx
{
    // NB:  My thinking here, is that if this dll is loaded once by the game,
    //      then only one static value will be held here. Last in, first out.
    //      However, if the game creates aliases, and loads each unpack directory
    //      separately, then there will be no issues. The VMods Core seemed to
    //      work as intended, so this might not be an issues. However, it is
    //      something to perform integration testing on, once we get to a
    //      stable, and testable state.

    /// <summary>
    ///     Gets the side designated within the mod information.
    /// </summary>
    /// <exception cref="GantryException">Cannot determine app-side before `ApiEx` is intialised.</exception>
    public static EnumAppSide ModAppSide { get; private set; }

    /// <summary>
    ///     The main assembly for the mod that initialised the Gantry MDK.
    /// </summary>
    /// HACK: Potential Integration Bug: Is this global, static object shared among disparate mods?
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
        Mod = Ensure.PopulatedWith(Mod, mod);
        ModAssembly = Ensure.PopulatedWith(ModAssembly, modAssembly);
        ModInfo = Ensure.PopulatedWith(ModInfo, mod.Info);

        if (ModInfo is null)
            throw new GantryException(
                @$"Could not find mod information for assembly: {ModAssembly.FullName}. " +
                "Are you missing a modinfo.json file, or `ModInfoAttribute` declaration?");

        ModAppSide = ModInfo.Side;
    }

    /// <summary>
    ///     Determines whether a given mod is installed, and enabled, on the current app-side.
    /// </summary>
    /// <param name="modId">The mod identifier.</param>
    /// <returns><c>true</c> if the mod is enabled; otherwise, <c>false</c>.</returns>
    public static bool IsModEnabled(string modId)
    {
        return ApiEx.Current.ModLoader.IsModEnabled(modId);
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
        return thread.GetApartmentState() == ApartmentState.STA
               && !thread.IsBackground
               && !thread.IsThreadPoolThread
               && thread.IsAlive;
    }
}