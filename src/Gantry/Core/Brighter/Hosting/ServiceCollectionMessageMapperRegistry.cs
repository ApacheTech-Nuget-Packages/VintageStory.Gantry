using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using JetBrains.Annotations;

namespace Gantry.Core.Brighter.Hosting;

/// <summary>
/// When parsing for message mappers in assemblies, stores any found message mappers. A later step will add these to the message mapper registry
/// Not used directly
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ServiceCollectionMessageMapperRegistry
{
    private readonly IServiceCollection _serviceCollection;
    private readonly ServiceLifetime _lifetime;

    /// <summary>
    ///     Gets the mappers.
    /// </summary>
    /// <value>The mappers.</value>
    public Dictionary<Type, Type> Mappers { get; } = [];

    /// <summary>
    ///     Gets the asynchronous mappers.
    /// </summary>
    /// <value>The asynchronous mappers.</value>
    public Dictionary<Type, Type> AsyncMappers { get; } = [];

    /// <summary>
    ///  	Initialises a new instance of the <see cref="ServiceCollectionMessageMapperRegistry"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="lifetime">The lifetime.</param>
    public ServiceCollectionMessageMapperRegistry(IServiceCollection serviceCollection, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        _serviceCollection = serviceCollection;
        _lifetime = lifetime;
    }

    /// <summary>
    /// Register a mapper with the collection (generic version)
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to map</typeparam>
    /// <typeparam name="TMessageMapper">The type of the mapper</typeparam>
    public void Register<TRequest, TMessageMapper>() where TRequest : class, IRequest where TMessageMapper : class, IAmAMessageMapper<TRequest>
    {
        Add(typeof(TRequest), typeof(TMessageMapper));
    }

    /// <summary>
    /// Register a mapper with the collection (generic version)
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to map</typeparam>
    /// <typeparam name="TMessageMapper">The type of the mapper</typeparam>
    public void RegisterAsync<TRequest, TMessageMapper>() where TRequest : class, IRequest where TMessageMapper : class, IAmAMessageMapperAsync<TRequest>
    {
        AddAsync(typeof(TRequest), typeof(TMessageMapper));
    }

    /// <summary>
    /// Add a mapper to the collection
    /// </summary>
    /// <param name="message">The type of message to map</param>
    /// <param name="mapper">The type of the mapper</param>
    public void Add(Type message, Type mapper)
    {
        _serviceCollection.TryAdd(new ServiceDescriptor(mapper, mapper, _lifetime));
        Mappers.Add(message, mapper);
    }

    /// <summary>
    /// Add a mapper to the collection
    /// </summary>
    /// <param name="message">The type of message to map</param>
    /// <param name="mapper">The type of the mapper</param>
    public void AddAsync(Type message, Type mapper)
    {
        _serviceCollection.TryAdd(new ServiceDescriptor(mapper, mapper, _lifetime));
        AsyncMappers.Add(message, mapper);
    }
}