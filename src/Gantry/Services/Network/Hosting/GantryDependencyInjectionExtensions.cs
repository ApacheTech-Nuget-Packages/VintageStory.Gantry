using Gantry.Core.Extensions.Api;

namespace Gantry.Services.Network.Hosting;

/// <summary>
///     Extension methods to aid the registration of the Network service, into a Gantry MDK IOC Container.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class GantryDependencyInjectionExtensions
{
    /// <summary>
    ///     Adds the embedded resources service to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="api">The api to run game commands on.</param>
    /// <param name="options">The services collection to add the service to.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddNetworkService(this IServiceCollection services, ICoreAPI api, Action<NetworkServiceOptions> options = null)
    {
        var service = new GantryNetworkService(api, NetworkServiceOptions.Default.With(options));
        services.AddSingleton<IUniversalNetworkService>(service);
        api.Side.RunOneOf(
            () => services.AddSingleton<IClientNetworkService>(service), 
            () => services.AddSingleton<IServerNetworkService>(service));
        return services;
    }
}