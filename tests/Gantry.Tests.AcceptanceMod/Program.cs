using ApacheTech.Common.DependencyInjection.Abstractions;
using Gantry.Core.DependencyInjection;
using Gantry.Services.FileSystem.DependencyInjection;
using Gantry.Services.HarmonyPatches.DependencyInjection;
using Gantry.Services.Network.DependencyInjection;

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
        protected override void ConfigureUniversalModServices(IServiceCollection services)
        {
            services.AddFileSystemService(o => o.RegisterSettingsFiles = true);
            services.AddHarmonyPatchingService(o => o.AutoPatchModAssembly = true);
            services.AddNetworkService();
        }
    }
}