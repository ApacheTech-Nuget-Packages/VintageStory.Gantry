using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.BrighterSlim.FeatureSwitch;

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Provides options for the BrighterSlim command processor.
/// </summary>
internal class BrighterOptions : IBrighterOptions
{
    /// <summary>
    ///     Configures the lifetime of the Command Processor. Defaults to Transient.
    /// </summary>
    public ServiceLifetime CommandProcessorLifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    ///     Do we support feature switching? In which case please supply an initialized feature switch registry
    /// </summary>
    /// <returns></returns>
    public IAmAFeatureSwitchRegistry FeatureSwitchRegistry { get; set; } = null;

    /// <summary>
    ///     Configures the lifetime of the Handlers. Defaults to Transient.
    /// </summary>
    public ServiceLifetime HandlerLifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    ///     Configures the lifetime of mappers. Defaults to Singleton
    /// </summary>
    public ServiceLifetime MapperLifetime { get; set; } = ServiceLifetime.Singleton;

    /// <summary>
    ///     Configures the request context factory. Defaults to <see cref="InMemoryRequestContextFactory" />.
    /// </summary>
    public IAmARequestContextFactory RequestContextFactory { get; set; } = new InMemoryRequestContextFactory();

    /// <summary>
    ///     Configures the lifetime of any transformers. Defaults to Singleton
    /// </summary>
    public ServiceLifetime TransformerLifetime { get; set; } = ServiceLifetime.Singleton;
}