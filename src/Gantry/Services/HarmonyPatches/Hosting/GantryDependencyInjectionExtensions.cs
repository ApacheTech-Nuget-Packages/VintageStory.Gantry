using Gantry.Core.Abstractions;

namespace Gantry.Services.HarmonyPatches.Hosting;

/// <summary>
///     Extension methods to aid the registration of the Harmony Patching service, into a Gantry MDK IOC Container.
/// </summary>
public static class GantryDependencyInjectionExtensions
{
    /// <summary>
    ///     Adds the embedded resources service to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add the service to.</param>
    /// <param name="core">Provides access to the core Gantry API.</param>
    /// <param name="options">The options to pass to the service.</param>
    /// <returns>A reference to this instance, after this operation has completed.</returns>
    public static IServiceCollection AddHarmonyPatchingService(this IServiceCollection services, ICoreGantryAPI core,
        Action<HarmonyPatchingServiceOptions>? options = null)
    {
        var harmonyOptions = HarmonyPatchingServiceOptions.Default(core).With(options);
        services.TryAddSingleton<IHarmonyPatchingService>(new HarmonyPatchingService(core, harmonyOptions));
        return services;
    }
}