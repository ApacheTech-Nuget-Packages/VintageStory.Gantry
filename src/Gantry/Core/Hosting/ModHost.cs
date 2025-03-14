using ApacheTech.Common.DependencyInjection;
using ApacheTech.Common.DependencyInjection.Extensions;
using Gantry.Core.Hosting.Extensions;
using Gantry.Core.Hosting.Registration;
using Gantry.Core.Hosting.Registration.Api;
using Gantry.Core.ModSystems.Abstractions;
using Gantry.Services.Brighter.Hosting;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Hosting;
using Gantry.Services.HarmonyPatches;
using Gantry.Services.HarmonyPatches.Hosting;
using Gantry.Services.Network.Hosting;
using Humanizer;
using Humanizer.Localisation;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Gantry.Core.Hosting;

/// <summary>
///     Only one derived instance of this base-class should be added to any single mod within
///     the VintageMods domain. This base-class will enable Dependency Injection, and add all
///     the domain services. Derived instances should only have minimal functionality, 
///     instantiating, and adding Application specific services to the IOC Container.
/// </summary>
/// <seealso cref="ModSystem" />
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public abstract class ModHost : GantrySubsytemHost
{
    private ICoreAPI _api;
    private readonly IServiceCollection _services;
    private List<IUniversalServiceRegistrar> _universalServiceRegistrars;
    private List<IServerServiceRegistrar> _serverServiceRegistrars;
    private List<IClientServiceRegistrar> _clientServiceRegistrars;

    /// <summary>
    ///     Initialises a new instance of the <see cref="ModHost" /> class.
    /// </summary>
    protected ModHost()
    {
#if DEBUG
        //Harmony.DEBUG = true;
        RuntimeEnv.DebugOutOfRangeBlockAccess = true;
        ModEx.DebugMode = true;
#endif
        _services = new ServiceCollection();
    }

    #region Universal Configuration

    /// <inheritdoc />
    protected override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
    {
        services.AddNetworkService();
        G.Logger.VerboseDebug($"Registered {api.Side} Network Service");

        services.AddFileSystemService(o => o.RegisterSettingsFiles = true);
        G.Logger.VerboseDebug($"Registered {api.Side} FileSystem Service");

        services.AddHarmonyPatchingService(o => o.AutoPatchModAssembly = true);
        G.Logger.VerboseDebug($"Registered {api.Side} Harmony Service");

        base.ConfigureUniversalModServices(services, api);
    }

    private static void ConfigureBrighter(IServiceCollection services, ICoreAPI api)
    {
        var brighterBuilder = services.AddBrighter();
        brighterBuilder.AutoFromAssemblies(api);
        G.Logger.VerboseDebug($"ModHost: Registered {api.Side} Brighter Mediator Engine.");
    }

    #endregion

    #region Server Configuration

    private void BuildServerHost(ICoreServerAPI sapi)
    {
        //  1. Configure game API services.
        _services.With(ioc => ServerApiRegistrar.RegisterServerApiEndpoints(ioc, sapi));
        G.Logger.VerboseDebug("ModHost: Registered Server API Endpoints.");

        //  2. Register all ModSystems within the mod. Will also self-reference this ModHost. 
        _services.AddModSystems(EnumAppSide.Server);
        _services.AddServerSystems();
        G.Logger.VerboseDebug("ModHost: Registered Server ModSystems.");

        //  3. Delegate mod service configuration to derived class.
        _services
            .With(ioc => ConfigureBrighter(ioc, sapi))
            .With(ioc => ConfigureUniversalModServices(ioc, sapi))
            .With(ioc => ConfigureServerModServices(ioc, sapi));
        G.Logger.VerboseDebug("ModHost: Registered ModHost Services.");

        //  4. Register all features that need registering. 
        _universalServiceRegistrars.ForEach(x => x.ConfigureUniversalModServices(_services, sapi));
        G.Logger.VerboseDebug("ModHost: Registered Universal Services.");

        _serverServiceRegistrars.ForEach(x => x.ConfigureServerModServices(_services, sapi));
        G.Logger.VerboseDebug("ModHost: Registered Server Services.");

        //  5. Build IOC Container.
        IOC.ServerIOC = _services.BuildServiceProvider();
        G.Logger.VerboseDebug("ModHost: IOC.ServerIOC now populated.");

        // ONLY NOW ARE SERVICES AVAILABLE

        //  6. Delegate mod PreStart to derived class.
        StartPreServerSide(sapi);
        base.StartPreServerSide(sapi);
    }

    #endregion

    #region Client Configuration

    private void BuildClientHost(ICoreClientAPI capi)
    {
        //  1. Configure game API services.
        _services.With(ioc => ClientApiRegistrar.RegisterClientApiEndpoints(ioc, capi));

        //  2. Register all ModSystems within the mod. Will also self-reference this ModHost. 
        _services.AddModSystems(EnumAppSide.Client);
        _services.AddClientSystems();

        //  3. Delegate mod service configuration to derived class.
        _services
            .With(ioc => ConfigureBrighter(ioc, capi))
            .With(ioc => ConfigureUniversalModServices(ioc, capi))
            .With(ioc => ConfigureClientModServices(ioc, capi));

        //  4. Register all features that need registering. 
        _universalServiceRegistrars.ForEach(x => x.ConfigureUniversalModServices(_services, capi));
        _clientServiceRegistrars.ForEach(x => x.ConfigureClientModServices(_services, capi));

        //  5. Build IOC Container.
        IOC.ClientIOC = _services.BuildServiceProvider();
        G.Logger.VerboseDebug("ModHost: IOC.ClientIOC now populated.");

        // ONLY NOW ARE SERVICES AVAILABLE

        //  6. Delegate mod PreStart to derived class.
        StartPreClientSide(capi);
        base.StartPreClientSide(capi);
    }

    #endregion

    #region Boilerplate

    /// <summary>
    ///     Called during initial mod loading, called before any mod receives the call to Start().
    /// </summary>
    /// <param name="api">
    ///     Common API Components that are available on the server and the client.
    ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected sealed override void StartPreUniversal(ICoreAPI api)
    {
        _api = api;
        var stopwatch = Stopwatch.StartNew();

        api.Logger.Event($"Gantry - Loading {Mod.Info.Name}, by {Mod.Info.Authors[0]}");
        G.CreateLogger(api, Mod);

        ApiEx.Initialise(api);
        G.Logger.VerboseDebug("ModHost: Initialised ApiEx.");

        ModEx.Initialise(Mod, GetType().Assembly);
        G.Logger.VerboseDebug("ModHost: Initialised ModEx.");

        _universalServiceRegistrars = [.. Mod.Systems.OfType<IUniversalServiceRegistrar>()];
        G.Logger.VerboseDebug("ModHost: Populated Universal Mod Service Registrars.");
        switch (api)
        {
            case ICoreClientAPI capi:
                _clientServiceRegistrars = [.. Mod.Systems.OfType<IClientServiceRegistrar>()];
                G.Logger.VerboseDebug("ModHost: Populated Client Mod Service Registrars.");
                BuildClientHost(capi);
                break;
            case ICoreServerAPI sapi:
                _serverServiceRegistrars = [.. Mod.Systems.OfType<IServerServiceRegistrar>()];
                G.Logger.VerboseDebug("ModHost: Populated Server Mod Service Registrars.");
                BuildServerHost(sapi);
                break;
        }
        StartPreUniversalSide(api);
        base.StartPreUniversal(api);
        stopwatch.Stop();
        G.Logger.VerboseDebug($"ModHost Loaded in {stopwatch.Elapsed.Humanize(maxUnit: TimeUnit.Millisecond)}.");
    }

    /// <summary>
    ///     Side agnostic Start method, called after all mods received a call to StartPre().
    /// </summary>
    /// <param name="api">
    ///     Common API Components that are available on the server and the client.
    ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public sealed override void Start(ICoreAPI api)
    {
        base.Start(api);
        StartUniversal(api);
    }

    /// <summary>
    ///     Called during initial mod loading, called before any mod receives the call to Start().
    /// </summary>
    /// <param name="api">
    ///     Common API Components that are available on the server and the client.
    ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
    /// </param>
    public virtual void StartPreUniversalSide(ICoreAPI api) { }

    /// <summary>
    ///     Side agnostic Start method, called after all mods received a call to StartPre().
    /// </summary>
    /// <param name="api">
    ///     Common API Components that are available on the server and the client.
    ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
    /// </param>
    public virtual void StartUniversal(ICoreAPI api) { }

    /// <summary>
    ///     If you need mods to be executed in a certain order, adjust this method's return value.
    ///     The server will call each Mods Start() method the ascending order of each mod's execute order value.
    ///     And thus, as long as every mod registers it's event handlers in the Start() method, all event handlers
    ///     will be called in the same execution order. Default execute order of some survival mod parts.
    /// 
    ///     World Gen: 
    ///     - GenTerra: 0
    ///     - RockStrata: 0.1
    ///     - Deposits: 0.2
    ///     - Caves: 0.3
    ///     - BlockLayers: 0.4
    /// 
    ///     Asset Loading:
    ///     - Json Overrides loader: 0.05
    ///     - Load hardcoded mantle block: 0.1
    ///     - Block and Item Loader: 0.2
    ///     - Recipes (Smithing, Knapping, ClayForming, Grid recipes, Alloys) Loader: 1
    /// </summary>
    public sealed override double ExecuteOrder() => double.NegativeInfinity;

    /// <summary>
    ///     If this mod allows runtime reloading, you must implement this method to unregister any listeners / handlers
    /// </summary>
    public override void Dispose()
    {
        G.Logger.VerboseDebug("Disposing FileSystem Service");
        IOC.Services.Resolve<IHarmonyPatchingService>().Dispose();

        G.Logger.VerboseDebug("Disposing Harmony Service");
        IOC.Services.Resolve<IFileSystemService>().Dispose();

        G.Logger.VerboseDebug("Disposing IOC Provider");
        _services.Clear();
        IOC.Dispose();

        G.Logger.VerboseDebug("Disposing ApiEx");
        ApiEx.Dispose(_api);

        G.DisposeLogger(_api);
        ModEx.ModAssemblies.Foreach(p => p.NullifyOrphanedStaticMembers());
    }

    #endregion
}