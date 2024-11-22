using System.Reflection.PortableExecutable;
using ApacheTech.Common.DependencyInjection;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core.Brighter.Hosting;
using Gantry.Core.Extensions.Api;
using Gantry.Core.Hosting.Extensions;
using Gantry.Core.Hosting.Registration;
using Gantry.Core.Hosting.Registration.Api;
using Gantry.Core.ModSystems;
using Gantry.Core.ModSystems.Abstractions;
using Gantry.Services.FileSystem.Hosting;
using Gantry.Services.HarmonyPatches.Hosting;
using Gantry.Services.Network.Hosting;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

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
    private readonly IServiceCollection _services;
    private List<IUniversalServiceRegistrar> _universalServiceRegistrars;
    private List<IServerServiceRegistrar> _serverServiceRegistrars;
    private List<IClientServiceRegistrar> _clientServiceRegistrars;

    /// <summary>
    ///     Initialises a new instance of the <see cref="ModHost" /> class.
    /// </summary>
    protected ModHost(bool debugMode = false)
    {
#if DEBUG
        debugMode = true;
#endif
        RuntimeEnv.DebugOutOfRangeBlockAccess = debugMode;
        ModEx.DebugMode = debugMode;
        Harmony.DEBUG = debugMode;
        _services = new ServiceCollection();
    }

    #region Universal Configuration

    /// <summary>
    ///     Configures any services that need to be added to the IO Container, on the both app sides, equally.
    /// </summary>
    /// <param name="services">The as-of-yet un-built services container.</param>
    /// <param name="api">Access to the universal API.</param>
    protected virtual void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
    {
        services.AddFileSystemService(api, o => o.RegisterSettingsFiles = true);
        services.AddHarmonyPatchingService(api, o => o.AutoPatchModAssembly = true);
        services.AddNetworkService(api);
    }

    private static void ConfigureBrighter(IServiceCollection services, ICoreAPI api)
    {
        var brighterBuilder = services.AddBrighter();
        brighterBuilder.AutoFromAssemblies(api);
        api.Logger.GantryDebug("[Gantry] ModHost: Registered Brighter Mediator Engine.");
    }

    #endregion

    #region Server Configuration

    private void BuildServerHost(ICoreServerAPI sapi)
    {
        //  1. Configure game API services.
        _services.With(ioc => ServerApiRegistrar.RegisterServerApiEndpoints(ioc, sapi));
        sapi.Logger.GantryDebug("[Gantry] ModHost: Registered Server API Endpoints.");

        //  2. Register all ModSystems within the mod. Will also self-reference this ModHost. 
        _services.AddModSystems(EnumAppSide.Server);
        sapi.Logger.GantryDebug("[Gantry] ModHost: Registered Server ModSystems.");

        //  3. Delegate mod service configuration to derived class.
        _services
            .With(ioc => ConfigureBrighter(ioc, sapi))
            .With(ioc => ConfigureUniversalModServices(ioc, sapi))
            .With(ioc => ConfigureServerModServices(ioc, sapi));
        sapi.Logger.GantryDebug("[Gantry] ModHost: Registered ModHost Services.");

        //  4. Register all features that need registering. 
        _universalServiceRegistrars.ForEach(x => x.ConfigureUniversalModServices(_services, sapi));
        sapi.Logger.GantryDebug("[Gantry] ModHost: Registered Universal Services.");

        _serverServiceRegistrars.ForEach(x => x.ConfigureServerModServices(_services, sapi));
        sapi.Logger.GantryDebug("[Gantry] ModHost: Registered Server Services.");

        //  5. Build IOC Container.
        IOC.ServerIOC = _services.BuildServiceProvider();
        sapi.Logger.GantryDebug("[Gantry] ModHost: IOC.ServerIOC now populated.");

        // ONLY NOW ARE SERVICES AVAILABLE

        //  6. Delegate mod PreStart to derived class.
        StartPreServerSide(sapi);
    }

    /// <summary>
    ///     Configures any services that need to be added to the IO Container, on the server side.
    /// </summary>
    /// <param name="services">The as-of-yet un-built services container.</param>
    /// <param name="sapi">Access to the server-side API.</param>
    protected virtual void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi) { }

    #endregion

    #region Client Configuration

    private void BuildClientHost(ICoreClientAPI capi)
    {
        //  1. Configure game API services.
        _services.With(ioc => ClientApiRegistrar.RegisterClientApiEndpoints(ioc, capi));

        //  2. Register all ModSystems within the mod. Will also self-reference this ModHost. 
        _services.AddModSystems(EnumAppSide.Client);

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

        // ONLY NOW ARE SERVICES AVAILABLE

        //  6. Delegate mod PreStart to derived class.
        StartPreClientSide(capi);
    }

    /// <summary>
    ///     Configures any services that need to be added to the IO Container, on the client side.
    /// </summary>
    /// <param name="services">The as-of-yet un-built services container.</param>
    /// <param name="capi">Access to the client-side API.</param>
    protected virtual void ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi) { }

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
        ModEx.Initialise(Mod, GetType().Assembly);
        api.Logger.GantryDebug("[Gantry] ModHost: Initialised ModEx.");

        ApiEx.Initialise(api);
        api.Logger.GantryDebug("[Gantry] ModHost: Initialised ApiEx.");

        _universalServiceRegistrars = Mod.Systems.OfType<IUniversalServiceRegistrar>().ToList();
        api.Logger.GantryDebug("[Gantry] ModHost: Populated Universal Mod Service Registrars.");
        switch (api)
        {
            case ICoreClientAPI capi:
                _clientServiceRegistrars = Mod.Systems.OfType<IClientServiceRegistrar>().ToList();
                api.Logger.GantryDebug("[Gantry] ModHost: Populated Client Mod Service Registrars.");
                BuildClientHost(capi);
                break;
            case ICoreServerAPI sapi:
                _serverServiceRegistrars = Mod.Systems.OfType<IServerServiceRegistrar>().ToList();
                api.Logger.GantryDebug("[Gantry] ModHost: Populated Server Mod Service Registrars.");
                BuildServerHost(sapi);
                break;
        }
        StartPreUniversalSide(api);
        base.StartPreUniversal(api);
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
        StartUniversal(api);
        base.Start(api);
        if (api is not ICoreClientAPI capi) return;
        capi.Event.LeftWorld += Dispose;
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
    public override double ExecuteOrder() => double.MinValue;

    /// <summary>
    ///     If this mod allows runtime reloading, you must implement this method to unregister any listeners / handlers
    /// </summary>
    public override void Dispose()
    {
        DisposeOnLeaveWorld();
        base.Dispose();
    }

    private static void DisposeOnLeaveWorld()
    {
        (IOC.Services as IDisposable)?.Dispose();
        ModEx.ModAssembly.NullifyOrphanedStaticMembers();
    }

    #endregion
}