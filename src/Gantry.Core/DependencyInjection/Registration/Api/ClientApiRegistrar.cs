using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Vintagestory.API.Common;
using Vintagestory.Client;
using Vintagestory.Client.NoObf;

namespace Gantry.Core.DependencyInjection.Registration.Api
{
    /// <summary>
    ///     Handles registration of the Game's API within the client-side IOC Container.
    /// </summary>
    internal static class ClientApiRegistrar
    {
        /// <summary>
        ///     Registers the Game's API within the client-side IOC Container.
        /// </summary>
        /// <param name="services">The IOC container.</param>
        public static void RegisterClientApiEndpoints(IServiceCollection services)
        {
            services.AddSingleton(ApiEx.Client);
            services.AddSingleton(ApiEx.Client.World);
            services.AddSingleton(ApiEx.ClientMain);
            services.AddSingleton(ApiEx.Current as ICoreAPICommon);
            services.AddSingleton(ApiEx.Current);
            services.AddSingleton(ScreenManager.Platform);
            services.AddSingleton(ScreenManager.Platform as ClientPlatformWindows);
        }
    }
}