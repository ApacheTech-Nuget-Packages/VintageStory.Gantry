using ApacheTech.Common.DependencyInjection.Abstractions;
using JetBrains.Annotations;
using Vintagestory.API.Server;

namespace Gantry.Core.DependencyInjection.Registration;

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
    /// <param name="sapi">Access to the server-side API.</param>
    void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi);
}