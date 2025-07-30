using Gantry.Core.Abstractions;
using Gantry.Core.Diagnostics;
using Gantry.Core.Helpers;
using Gantry.Core.Hosting.Registration;
using Gantry.Services.IO.Abstractions.Contracts;
using Vintagestory.Common;

namespace Gantry.Core;

/// <summary>
///     Default implementation of the <see cref="ICoreGantryAPI"/> interface, providing access to the core Gantry API features.
/// </summary>
internal class GantryCore : ICoreGantryAPI
{
    private class GantryServerCore(ICoreAPI api, ModContainer mod) : GantryCore(api, mod);
    private class GantryClientCore(ICoreAPI api, ModContainer mod) : GantryCore(api, mod);

    /// <summary>
    ///     Initialises a new instance of the <see cref="GantryCore"/> class.
    /// </summary>
    /// <param name="api">The core API instance, provided by the game engine.</param>
    /// <param name="mod">The mod instance, which is the host for this Gantry core.</param>
    internal static GantryCore Create(ICoreAPI api, ModContainer mod)
    {
        return api.Side == EnumAppSide.Server
            ? new GantryServerCore(api, mod)
            : new GantryClientCore(api, mod);
    }

    private GantryCore(ICoreAPI api, ModContainer mod)
    {
        Uapi = api;
        Mod = mod;
        ModAssembly = mod.Assembly;
        Logger = GantryLogger.Create(api.Side, mod.Logger, mod.Info);
        HolaMundo();
        Logger.VerboseDebug($"GantryCore: Version: {mod.Info.Version}");
        Logger.VerboseDebug($"GantryCore: Mod Assembly: {ModAssembly.FullName}");
        ApiEx = new ModApiContext(this);
        Logger.VerboseDebug($"GantryCore: ApiEx now online");
        Lang = new StringTranslator(mod.Info.ModID);
        Logger.VerboseDebug($"GantryCore: Language Translation Service now online (Default Domain: {Lang.DefaultDomain})");
    }

    private void HolaMundo()
    {
        var message = "¡Hola, Mundo!";
        Logger.Audit(message);
        Logger.Build(message);
        Logger.Chat(message);
        Logger.Debug(message);
        Logger.Error(message);
        Logger.Event(message);
        Logger.Notification(message);
        Logger.Fatal(message);
        Logger.StoryEvent(message);
        Logger.VerboseDebug(message);
        Logger.Warning(message);
    }

    internal void BuildServiceProvider(GantryHostOptions options)
    {
        Services = GantryServiceCollection.BuildHost(this, options);
        if (options.RegisterSettingsFiles)
            Services
                .GetRequiredService<IFileSystemService>()
                .RegisterDefaultSettingsFiles();
    }

    /// <inheritdoc />
    public ILogger Logger { get; }

    /// <inheritdoc />
    public Mod Mod { get; }

    /// <inheritdoc />
    public Assembly ModAssembly { get; }
    
    /// <inheritdoc />
    public IModApiContext ApiEx { get; }
    
    /// <inheritdoc />
    public IStringTranslator Lang { get; }
    
    /// <inheritdoc />
    public IServiceProvider Services { get; private set; } = default!;

    /// <inheritdoc />
    public ICoreAPI Uapi { get; }
}