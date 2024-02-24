using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using JetBrains.Annotations;

namespace Gantry.Services.Network.DependencyInjection;

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
    /// <param name="options">The services collection to add the service to.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddNetworkService(this IServiceCollection services, Action<NetworkServiceOptions> options)
    {
        var service = new GantryNetworkService(ApiEx.Current, NetworkServiceOptions.Default.With(options));
        services.AddSingleton<IUniversalNetworkService>(service);
        ApiEx.Side.RunOneOf(
            () => services.AddSingleton<IClientNetworkService>(service),
            () => services.AddSingleton<IServerNetworkService>(service));
        return services;
    }

    /// <summary>
    ///     Adds the embedded resources service to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddNetworkService(this IServiceCollection services)
    {
        return services.AddNetworkService(_ => { });
    }
}