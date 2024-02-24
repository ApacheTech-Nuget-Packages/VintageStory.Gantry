using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace Gantry.Core.DependencyInjection.Registration.Api;

/// <summary>
///     Handles registration of the Game's API within the server-side IOC Container.
/// </summary>
internal static class ServerApiRegistrar
{
    /// <summary>
    ///     Registers the Game's API within the server-side IOC Container.
    /// </summary>
    /// <param name="services">The IOC container.</param>
    /// <param name="sapi">The Server-Side API to register.</param>
    public static void RegisterServerApiEndpoints(IServiceCollection services, ICoreServerAPI sapi)
    {
        services.AddSingleton(sapi);
        services.AddSingleton(sapi.World);
        services.AddSingleton((sapi.World as ServerMain)!);
        services.AddSingleton(sapi as ICoreAPICommon);
        services.AddSingleton(sapi as ICoreAPI);
    }
}