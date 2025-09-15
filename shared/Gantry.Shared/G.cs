using Gantry.Services.EasyX.Hosting;

/// <summary>
///     Provides the core API surface for Gantry mods, exposing logging, dependency injection, localisation, mod metadata, and core services.
///     This interface is implemented by the Gantry core and injected into mod hosts and services, ensuring correct context and isolation per mod.
/// </summary>
internal static partial class G
{
    private readonly static Sided<ICoreGantryAPI> _sidedCore = Sided<ICoreGantryAPI>.AsyncLocal()!;

    internal static void SetCore(ICoreGantryAPI core) 
        => _sidedCore.Set(core.Side, core);

    internal static ICoreGantryAPI Core 
        => _sidedCore.Current!;

    /// <summary>
    ///     Logger for diagnostic and debug output.
    /// </summary>
    internal static ILogger Logger 
        => Core.Logger;

    /// <summary>
    ///     The underlying Vintagestory API instance for the current mod.
    /// </summary>
    internal static ICoreAPI Uapi 
        => Core.ApiEx.Current;

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
    ///     Returns a localised string for a feature-specific path in the current mod domain.
    /// </summary>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <param name="args">Optional arguments for formatting.</param>
    /// <returns>The localised string for the feature path.</returns>
    internal static string T(string featureName, string code, params object[] args)
        => Lang.Translate(featureName, code, args);

    /// <summary>
    ///     The dependency injection service provider for the mod.
    /// </summary>
    internal static IServiceProvider Services
        => Core.Services;

    /// <summary>
    ///     Harmony patching service for runtime method interception and patch management.
    /// </summary>
    internal static IHarmonyPatchingService Harmony 
        => Services.GetRequiredService<IHarmonyPatchingService>();

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
    ///     Provides access to the game world, including blocks, entities, and environment data.
    /// </summary>
    public static IWorldAccessor World
        => Uapi.World;

    /// <summary>
    ///     Writes a verbose debug message to the mod logger.
    /// </summary>
    /// <param name="messageTemplate">The message template to log.</param>
    /// <param name="args">Arguments for formatting the message.</param>
    internal static void Log(string messageTemplate, params object[] args)
        => Logger.VerboseDebug(messageTemplate, args);

    internal static void Dispose() 
        => _sidedCore.Dispose(Side);

    /// <summary>
    ///     An abstract base class for creating a mod host that integrates with the Gantry core API.
    /// </summary>
    /// <typeparam name="TProgram">The type of the main entry point for the mod.</typeparam>
    internal abstract class Host<TProgram> : ModHost<TProgram> where TProgram : Host<TProgram>
    {
        /// <inheritdoc />
        protected sealed override void OnCoreLoaded(ICoreGantryAPI core) => SetCore(core);

        /// <inheritdoc />
        protected override void OnCoreUnloaded() => G.Dispose();
    }

    /// <summary>
    ///     An abstract base class for creating a mod host that integrates with the Gantry core API.
    /// </summary>
    /// <typeparam name="TProgram">The type of the main entry point for the mod.</typeparam>
    /// <param name="commandName">The chat command name to register for server-side settings management.</param>
    internal abstract class ExHost<TProgram>(string commandName) : EasyXHost<TProgram>(commandName) where TProgram : ExHost<TProgram>
    {
        /// <inheritdoc />
        protected sealed override void OnCoreLoaded(ICoreGantryAPI core) => SetCore(core);

        /// <inheritdoc />
        protected override void OnCoreUnloaded() => G.Dispose();
    }
}