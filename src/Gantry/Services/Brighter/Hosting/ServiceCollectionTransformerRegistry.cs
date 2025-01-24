namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     This class helps us register transformers with the IoC container
///     We don't have a separate registry for transformers, but we do need to understand
///     the service lifetime options for the transformers which we want to register
/// </summary>
internal class ServiceCollectionTransformerRegistry : ITransformerRegistry
{
    private readonly IServiceCollection _services;
    private readonly ServiceLifetime _serviceLifetime;

    /// <summary>
    ///     Constructs a new instance
    /// </summary>
    /// <param name="services">The Service Collection to register the transforms with</param>
    /// <param name="serviceLifetime">The lifetime to use for registration. Defaults to Singleton</param>
    public ServiceCollectionTransformerRegistry(IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        _services = services;
        _serviceLifetime = serviceLifetime;
    }

    /// <summary>
    ///     Register a transform with the IServiceCollection using the ServiceLifetime
    /// </summary>
    /// <param name="transform">The type of the transform to register</param>
    public void Add(Type transform)
    {
        _services.TryAdd(new ServiceDescriptor(transform, transform, _serviceLifetime));
    }
}