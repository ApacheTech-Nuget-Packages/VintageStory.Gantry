using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Helpers;
using Gantry.Services.HarmonyPatches;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.Configuration;

namespace Gantry.Core.Abstractions;

/// <summary>
///     Provides the core API surface for Gantry mods, exposing logging, dependency injection, localisation, mod metadata, and core services.
///     This interface is implemented by the Gantry core and injected into mod hosts and services, ensuring correct context and isolation per mod.
/// </summary>
public interface ICoreGantryAPI
{
    /// <summary>
    ///     Logger for diagnostic and debug output.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    ICoreAPI Uapi { get; }

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    ICoreClientAPI Capi => ApiEx.Client;

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    ICoreServerAPI Sapi => ApiEx.Server;

    /// <summary>
    ///     The current mod, as registered with the mod manager.
    /// </summary>
    Mod Mod { get; }

    /// <summary>
    ///     The main assembly for the mod that initialised the Gantry MDK.
    /// </summary>
    Assembly ModAssembly { get; }

    /// <summary>
    ///     The main assemblies for the mod, including the Gantry MDK, if not merged into the main assembly.
    /// </summary>
    List<Assembly> ModAssemblies => [.. new[] { typeof(IModHost).Assembly, ModAssembly }.Distinct()];

    /// <summary>
    ///     Provides access to the Gantry mod API context, including side-specific helpers and utilities.
    /// </summary>
    IModApiContext ApiEx { get; }

    /// <summary>
    ///     The current application side (client, server, or universal).
    /// </summary>
    EnumAppSide Side => ApiEx.Side;

    /// <summary>
    ///     Provides localisation and translation services for the mod.
    /// </summary>
    IStringTranslator Lang { get; }

    /// <summary>
    ///     Harmony patching service for runtime method interception and patch management.
    /// </summary>
    IHarmonyPatchingService Harmony => Services.GetRequiredService<IHarmonyPatchingService>();

    /// <summary>
    ///     The dependency injection service provider for the mod.
    /// </summary>
    IServiceProvider Services { get; }

    /// <summary>
    ///     Brighter command processor for CQRS and messaging patterns.
    /// </summary>
    IAmACommandProcessor CommandProcessor => Services.GetRequiredService<IAmACommandProcessor>();

    /// <summary>
    ///     File system service for mod data and configuration file access.
    /// </summary>
    IFileSystemService IO => Services.GetRequiredService<IFileSystemService>();

    /// <summary>
    ///     Mod settings service for configuration and settings management.
    /// </summary>
    IModSettingsService Settings => Services.GetRequiredService<IModSettingsService>();

    /// <summary>
    ///     Writes a verbose debug message to the mod logger.
    /// </summary>
    /// <param name="messageTemplate">The message template to log.</param>
    /// <param name="args">Arguments for formatting the message.</param>
    public void Log(string messageTemplate, params object[] args)
        => Logger.VerboseDebug(messageTemplate, args);
}