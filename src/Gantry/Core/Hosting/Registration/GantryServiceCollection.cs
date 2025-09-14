using ApacheTech.Common.DependencyInjection;
using ApacheTech.Common.DependencyInjection.Extensions;
using Gantry.Core.Abstractions;
using Gantry.Core.Hosting.Extensions;
using Gantry.Extensions.Api;
using Gantry.Services.Brighter.Hosting;
using Gantry.Services.HarmonyPatches.Hosting;
using Gantry.Services.IO.Hosting;
using Vintagestory.Client;

namespace Gantry.Core.Hosting.Registration;

internal sealed class GantryServiceCollection : ServiceCollection
{
    private static GantryHostOptions _options = GantryHostOptions.Default;

    internal static IServiceProvider BuildHost(ICoreGantryAPI gantry, GantryHostOptions? options = null)
    {
        _options = options ?? GantryHostOptions.Default;
        var services = new GantryServiceCollection();
        services.AddSingleton<IServiceCollection>(services);

        //  1. Register all ModSystems within the mod. Will also self-reference the GantryCore. 
        gantry.Logger.Highlight($"BuildHost: Building {gantry.Side}-Side Service Collection");
        gantry.Log("BuildHost: Adding Core Gantry Services.");
        RegisterGantryServices(services, gantry);
        services.AddModSystems(gantry);
        services.AddSystems(gantry);

        //  2. Delegate mod service configuration to derived class.
        gantry.Log("BuildHost: Adding Core Universal Services.");
        services
            .With(ioc => ConfigureBrighter(ioc, gantry))
            .With(ioc => ConfigureUniversalModServices(ioc, gantry));

        //  3. Register all features that need registering.
        gantry.Log("BuildHost: Adding Mod-Specific Universal Services.");
        var universalServiceRegistrars = gantry.Mod.Systems.OfType<IUniversalServiceRegistrar>().ToList();
        universalServiceRegistrars.ForEach(x => x.ConfigureUniversalModServices(services, gantry));

        //  4. Register API Endpoints.
        gantry.ApiEx.Run(
        () =>
        {
            gantry.Log("BuildHost: Adding Client Services.");
            RegisterClientApiEndpoints(services, gantry);
            var clientServiceRegistrars = gantry.Mod.Systems.OfType<IClientServiceRegistrar>().ToList();
            clientServiceRegistrars.ForEach(x => x.ConfigureClientModServices(services, gantry));
        },
        () =>
        {
            gantry.Log("BuildHost: Adding Server Services.");
            RegisterServerApiEndpoints(services, gantry);
            var serverServiceRegistrars = gantry.Mod.Systems.OfType<IServerServiceRegistrar>().ToList();
            serverServiceRegistrars.ForEach(x => x.ConfigureServerModServices(services, gantry));
        });

        //  5. Build IOC Container.
        IServiceProvider serviceProvider = null!;
        services.AddSingleton(serviceProvider = services.BuildServiceProvider(o => o.DisposableAssemblies = gantry.ModAssemblies));
        gantry.Logger.Highlight($"BuildHost: ServiceProvider built with {services.Count} services");

        return serviceProvider;
    }

    private static void RegisterGantryServices(IServiceCollection services, ICoreGantryAPI gantry)
    {
        services.AddSingleton(gantry);
        services.AddSingleton(gantry.Mod);
        services.AddSingleton(gantry.Lang);
        services.AddSingleton(gantry.Logger);
        services.AddSingleton(gantry.ApiEx);
        gantry.Log($" - Gantry Core Services");
    }

    private static void RegisterServerApiEndpoints(IServiceCollection services, ICoreGantryAPI gantry)
    {
        var sapi = gantry.Uapi.To<ICoreServerAPI>();
        services.AddSingleton(sapi);
        services.AddSingleton(sapi.World);
        services.AddSingleton(sapi.World.To<ServerMain>());
        services.AddSingleton(sapi as ICoreAPICommon);
        services.AddSingleton(sapi as ICoreAPI);
        services.AddSingleton(sapi.ChatCommands);
        services.AddSingleton(sapi.Event);
        services.AddSingleton(sapi.Network);
        gantry.Log($" - Server API Endpoints");
    }

    private static void RegisterClientApiEndpoints(IServiceCollection services, ICoreGantryAPI gantry)
    {
        var capi = gantry.Uapi.To<ICoreClientAPI>();
        services.AddSingleton(capi);
        services.AddSingleton(capi.World);
        services.AddSingleton(capi.World.To<ClientMain>());
        services.AddSingleton(capi as ICoreAPICommon);
        services.AddSingleton(capi as ICoreAPI);
        services.AddSingleton(ScreenManager.Platform);
        services.AddSingleton(ScreenManager.Platform.To<ClientPlatformWindows>());
        services.AddSingleton(capi.ChatCommands);
        services.AddSingleton(capi.Event);
        services.AddSingleton(capi.Network);
        gantry.Log($" - Client API Endpoints");
    }

    private static void ConfigureUniversalModServices(IServiceCollection services, ICoreGantryAPI gantry)
    {
        gantry.Log($" - FileSystem Service");
        services.AddFileSystemService(o =>
        {
            o.RegisterSettingsFiles = _options.RegisterSettingsFiles;
        });

        gantry.Log($" - Harmony Patching Service");
        services.AddHarmonyPatchingService(gantry, o => o.AutoPatchModAssembly = _options.ApplyPatches);
    }

    private static void ConfigureBrighter(IServiceCollection services, ICoreGantryAPI gantry)
    {
        var brighterBuilder = services.AddBrighter();
        brighterBuilder.AutoFromAssemblies(gantry);
        gantry.Log($" - Brighter Mediator Engine");
    }
}