using Vintagestory.Client;

namespace Gantry.Core.Hosting.Registration.Api;

/// <summary>
///     Handles registration of the Game's API within the client-side IOC Container.
/// </summary>
internal static class ClientApiRegistrar
{
    /// <summary>
    ///     Registers the Game's API within the client-side IOC Container.
    /// </summary>
    /// <param name="services">The IOC container.</param>
    /// <param name="capi">Access to the client-side API.</param>
    public static void RegisterClientApiEndpoints(IServiceCollection services, ICoreClientAPI capi)
    {
        services.AddSingleton(capi);
        services.AddSingleton(capi.World);
        services.AddSingleton((capi.World as ClientMain)!);
        services.AddSingleton(capi as ICoreAPICommon);
        services.AddSingleton(capi as ICoreAPI);
        services.AddSingleton(ScreenManager.Platform);
        services.AddSingleton((ScreenManager.Platform as ClientPlatformWindows)!);
    }
}