using ApacheTech.Common.DependencyInjection.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Core.DependencyInjection.Registration
{
    /// <summary>
    ///     Represents a class that can add services to the Client IOC container.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public interface IServerServiceRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureServerModServices(IServiceCollection services);
    }
}