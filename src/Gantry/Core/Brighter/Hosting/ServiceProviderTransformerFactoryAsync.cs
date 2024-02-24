using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;

namespace Gantry.Core.Brighter.Hosting;

/// <summary>
/// A factory for creating transformers, backed by the .NET Service Collection
/// </summary>
public class ServiceProviderTransformerFactoryAsync : IAmAMessageTransformerFactoryAsync
{
    private readonly IServiceProvider _serviceProvider;
    private readonly bool _isTransient;

    /// <summary>
    /// Constructs a transformer factory
    /// </summary>
    /// <param name="serviceProvider">The IoC container we use to satisfy requests for transforms</param>
    public ServiceProviderTransformerFactoryAsync(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var options = serviceProvider.Resolve<IBrighterOptions>();
        if (options == null) _isTransient = false; else _isTransient = options.HandlerLifetime == ServiceLifetime.Transient;
    }

    /// <summary>
    /// Creates a specific transformer on demand
    /// </summary>
    /// <param name="transformerType">The type of transformer to create</param>
    /// <returns></returns>
    public IAmAMessageTransformAsync Create(Type transformerType)
    {
        return (IAmAMessageTransformAsync)_serviceProvider.GetService(transformerType);
    }

    /// <summary>
    /// If the transform was scoped as transient, we release it when the pipeline is finished
    /// </summary>
    /// <param name="transformer"></param>
    public void Release(IAmAMessageTransformAsync transformer)
    {
        if (!_isTransient) return;

        var disposal = transformer as IDisposable;
        disposal?.Dispose();
    }
}