using System;
using ApacheTech.Common.DependencyInjection.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Core.DependencyInjection.Registration
{
    /// <summary>
    ///     Represents a class that can add services to the Client IOC container. Implements <see cref="IDisposable" />.
    /// </summary>
    /// <seealso cref="IDisposable" />
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public interface IClientServiceRegistrar : IDisposable
    {
        /// <summary>
        ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureClientModServices(IServiceCollection services);
    }
}