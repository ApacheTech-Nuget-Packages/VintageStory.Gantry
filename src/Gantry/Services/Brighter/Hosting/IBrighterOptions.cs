using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.BrighterSlim.FeatureSwitch;
using Polly.Registry;

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Provides options for the BrighterSlim command processor.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal interface IBrighterOptions
{
    /// <summary>
    ///     Configures the lifetime of the Command Processor.
    /// </summary>
    ServiceLifetime CommandProcessorLifetime { get; set; }

    /// <summary>
    ///     Do we support feature switching? In which case please supply an initialized feature switch registry
    /// </summary>
    IAmAFeatureSwitchRegistry FeatureSwitchRegistry { get; set; }

    /// <summary>
    ///     Configures the lifetime of the Handlers.
    /// </summary>
    ServiceLifetime HandlerLifetime { get; set; }

    /// <summary>
    ///     Configures the lifetime of mappers. 
    /// </summary>
    ServiceLifetime MapperLifetime { get; set; }

    /// <summary>
    ///     Configures the polly policy registry.
    /// </summary>
    IPolicyRegistry<string> PolicyRegistry { get; set; }

    /// <summary>
    ///     Configures the request context factory. Defaults to <see cref="InMemoryRequestContextFactory" />.
    /// </summary>
    IAmARequestContextFactory RequestContextFactory { get; set; }

    /// <summary>
    ///     Configures the lifetime of any transformers.
    /// </summary>
    ServiceLifetime TransformerLifetime { get; set; }
}