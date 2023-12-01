using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using JetBrains.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Gantry.Services.HarmonyPatches.DependencyInjection;

/// <summary>
///     Extension methods to aid the registration of the Harmony Patching service, into a Gantry MDK IOC Container.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class GantryDependencyInjectionExtensions
{
    /// <summary>
    ///     Adds the embedded resources service to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="options">The options to pass to the service.</param>
    /// <returns>A reference to this instance, after the this operation has completed.</returns>
    public static IServiceCollection AddHarmonyPatchingService(
        this IServiceCollection services, 
        Action<HarmonyPatchingServiceOptions> options)
    {
        services.TryAddSingleton<IHarmonyPatchingService>(
            new HarmonyPatchingService(
                HarmonyPatchingServiceOptions.Default.With(options)));
        return services;
    }

    /// <summary>
    ///     Adds the embedded resources service to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <returns>A reference to this instance, after the this operation has completed.</returns>
    public static IServiceCollection AddHarmonyPatchingService(this IServiceCollection services)
    {
        return services.AddHarmonyPatchingService(_ => { });
    }
}