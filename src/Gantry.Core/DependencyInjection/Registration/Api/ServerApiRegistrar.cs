using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Vintagestory.API.Common;

namespace Gantry.Core.DependencyInjection.Registration.Api
{
    /// <summary>
    ///     Handles registration of the Game's API within the server-side IOC Container.
    /// </summary>
    internal static class ServerApiRegistrar
    {
        /// <summary>
        ///     Registers the Game's API within the server-side IOC Container.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        public static void RegisterServerApiEndpoints(IServiceCollection container)
        {
            container.AddSingleton(ApiEx.Server);
            container.AddSingleton(ApiEx.Server.World);
            container.AddSingleton(ApiEx.ServerMain);
            container.AddSingleton((ICoreAPICommon)ApiEx.Current);
            container.AddSingleton(ApiEx.Current);
        }
    }
}