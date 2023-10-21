using ApacheTech.Common.DependencyInjection.Abstractions;
using Gantry.Core.DependencyInjection;
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.HarmonyPatches.DependencyInjection;
using Gantry.Services.Network.DependencyInjection;
using Vintagestory.API.Common;

namespace Gantry.Tests.AcceptanceMod
{
    /// <summary>
    ///     Entry point to the mod.
    /// </summary>
    /// <seealso cref="ModHost" />
    public class Program : ModHost
    {
        /// <summary>
        ///     Configures any services that need to be added to the IO Container, on the both app sides, equally.
        /// </summary>
        /// <param name="services">The as-of-yet un-built services container.</param>
        /// <param name="api">The api for the current side.</param>
        protected override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
        {
            services.AddFileSystemService(o => o.RegisterSettingsFiles = true);
            services.AddHarmonyPatchingService(o => o.AutoPatchModAssembly = true);
            services.AddNetworkService();
        }
    }
}