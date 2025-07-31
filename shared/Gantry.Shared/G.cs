using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core;
using System.Reflection;

namespace Gantry;

/// <summary>
///     Provides the core API surface for Gantry mods, exposing logging, dependency injection, localisation, mod metadata, and core services.
///     This interface is implemented by the Gantry core and injected into mod hosts and services, ensuring correct context and isolation per mod.
/// </summary>
internal static class G
{
    private readonly static Sided<ICoreGantryAPI> _sidedCore = new();

    internal static void SetCore(ICoreGantryAPI core)
    {
        _sidedCore.Set(core.Side, core);
        Nexus.AddCore(core);
    }

    internal static ICoreGantryAPI Core => _sidedCore.Current!;

    /// <summary>
    ///     Logger for diagnostic and debug output.
    /// </summary>
    internal static ILogger Logger 
        => Core.Logger;

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    internal static ICoreAPI Uapi 
        => Core.Uapi;

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    internal static ICoreClientAPI Capi
        => Core.ApiEx.Client;

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    internal static ICoreServerAPI Sapi
        => Core.ApiEx.Server;

    /// <summary>
    ///     The current mod, as registered with the mod manager.
    /// </summary>
    internal static Mod Mod 
        => Core.Mod;

    /// <summary>
    ///     The main assembly for the mod that initialised the Gantry MDK.
    /// </summary>
    internal static Assembly ModAssembly
        => Core.ModAssembly;

    /// <summary>
    ///     The main assemblies for the mod, including the Gantry MDK, if not merged into the main assembly.
    /// </summary>
    internal static IEnumerable<Assembly> ModAssemblies 
        => new[] { typeof(ICoreGantryAPI).Assembly, ModAssembly }.Distinct();

    /// <summary>
    ///     Provides access to the Gantry mod API context, including side-specific helpers and utilities.
    /// </summary>
    internal static IModApiContext ApiEx 
        => Core.ApiEx;

    /// <summary>
    ///     The current application side (client, server, or universal).
    /// </summary>
    internal static EnumAppSide Side 
        => ApiEx.Side;

    /// <summary>
    ///     Provides localisation and translation services for the mod.
    /// </summary>
    internal static IStringTranslator Lang
        => Core.Lang;

    /// <summary>
    ///     Harmony patching service for runtime method interception and patch management.
    /// </summary>
    internal static IHarmonyPatchingService Harmony 
        => Services.GetRequiredService<IHarmonyPatchingService>();

    /// <summary>
    ///     The dependency injection service provider for the mod.
    /// </summary>
    internal static IServiceProvider Services 
        => Core.Services;

    /// <summary>
    ///     Brighter command processor for CQRS and messaging patterns.
    /// </summary>
    internal static IAmACommandProcessor CommandProcessor 
        => Services.GetRequiredService<IAmACommandProcessor>();

    /// <summary>
    ///     File system service for mod data and configuration file access.
    /// </summary>
    internal static IFileSystemService IO 
        => Services.GetRequiredService<IFileSystemService>();

    /// <summary>
    ///     Mod settings service for configuration and settings management.
    /// </summary>
    internal static IModSettingsService Settings
        => Services.GetRequiredService<IModSettingsService>();

    /// <summary>
    ///     Writes a verbose debug message to the mod logger.
    /// </summary>
    /// <param name="messageTemplate">The message template to log.</param>
    /// <param name="args">Arguments for formatting the message.</param>
    internal static void Log(string messageTemplate, params object[] args)
        => Logger.VerboseDebug(messageTemplate, args);
}