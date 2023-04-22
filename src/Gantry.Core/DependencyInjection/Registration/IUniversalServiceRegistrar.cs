using ApacheTech.Common.DependencyInjection.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Core.DependencyInjection.Registration
{
    /// <summary>
    ///     Represents a class that can add services to both the Client, and Server IOC containers.
    /// </summary>
    /// <seealso cref="IServerServiceRegistrar" />
    /// <seealso cref="IClientServiceRegistrar" />
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public interface IUniversalServiceRegistrar : IServerServiceRegistrar, IClientServiceRegistrar
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureUniversalModServices(IServiceCollection services);
    }
}