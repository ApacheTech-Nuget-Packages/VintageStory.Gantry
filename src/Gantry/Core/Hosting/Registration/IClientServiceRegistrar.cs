using Gantry.Core.Abstractions;

namespace Gantry.Core.Hosting.Registration;

/// <summary>
///     Represents a class that can add services to the Client IOC container. Implements <see cref="IDisposable" />.
/// </summary>
/// <seealso cref="IDisposable" />
public interface IClientServiceRegistrar : IDisposable
{
    /// <summary>
    ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="gantry">Access to the Core Gantry API.</param>
    void ConfigureClientModServices(IServiceCollection services, ICoreGantryAPI gantry);
}