using System.Reflection;
using Gantry.Core.Diagnostics;
using Gantry.Core.Extensions.Api;
using Gantry.Services.FileSystem.Extensions;

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
    private static ILogger _clientLogger;
    private static ILogger _serverLogger;

    internal static void Initialise(ICoreAPI api, Mod mod, Assembly modAssembly)
    {
        api.Logger.Debug("[Gantry] Initialising gantry loggers.");
        api.RunOneOf(
            capi => _clientLogger = GantryLogger.Create(capi, mod.Info), 
            sapi => _serverLogger = GantryLogger.Create(sapi, mod.Info));
        var logger = api.GetGantryLogger();

        logger.VerboseDebug($"Setting Mod details for: {mod.FileName}");
        Mod = Ensure.PopulatedWith(Mod, mod);

        logger.VerboseDebug($"Setting Mod assembly as: {modAssembly.FullName}");
        ModAssembly = Ensure.PopulatedWith(ModAssembly, modAssembly);

        logger.VerboseDebug($"Setting Mod Info for: {mod.Info.ModID}");
        ModInfo = Ensure.PopulatedWith(ModInfo, mod.Info);

        logger.VerboseDebug("Creating Initial ModData Directory");
        CreateInitialDirectory();
    }

    /// <summary>
    ///     Represents the current mod, as registered with the mod manager.
    /// </summary>
    public static Mod Mod { get; private set; }

    /// <summary>
    ///     The main assembly for the mod that initialised the Gantry MDK.
    /// </summary>
    public static Assembly ModAssembly { get; private set; }

    /// <summary>
    ///     The main assemblies for the mod, including the Gantry MDK.
    /// </summary>
    public static IEnumerable<Assembly> ModAssemblies => new[] { typeof(ModEx).Assembly, ModAssembly }.Distinct();

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
    ///     
    /// </summary>
    /// <param name="api"></param>
    /// <returns></returns>
    public static ILogger GetGantryLogger(this ICoreAPI api)
        => api.Side.ChooseOneOf(_clientLogger, _serverLogger);

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
}