using Gantry.Core.Hosting.Registration.Factories;

namespace Gantry.Core.Hosting.Extensions;

/// <summary>
///		Extension methods that extend the functionality of the dependency injection engine.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///		Apply the decorator pattern to the Dependency Injection container.
    /// </summary>
    /// <see href="https://greatrexpectations.com/2018/10/25/decorators-in-net-core-with-dependency-injection"/>
    /// <typeparam name="TInterface">The interface type to register a decorator for</typeparam>
    /// <typeparam name="TDecorator">The type of the decorator implementation</typeparam>
    public static void Decorate<TInterface, TDecorator>(this IServiceCollection services)
      where TInterface : class
      where TDecorator : class, TInterface
    {
        // grab the existing registration
        var wrappedDescriptor = services.FirstOrDefault(
          s => s.ServiceType == typeof(TInterface))
            ?? throw new InvalidOperationException($"{typeof(TInterface).Name} is not registered");

        // create the object factory for our decorator type,
        // specifying that we will supply TInterface explicitly
        var objectFactory = ActivatorUtilities.CreateFactory(typeof(TDecorator), [typeof(TInterface)]);

        // replace the existing registration with one
        // that passes an instance of the existing registration
        // to the object factory for the decorator
        services.Replace(ServiceDescriptor.Describe(
          typeof(TInterface),
          s => (TInterface)objectFactory(s, [s.CreateInstance(wrappedDescriptor)]),
          wrappedDescriptor.Lifetime)
        );
    }

    private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.Implementation is not null)
            return descriptor.Implementation;

        if (descriptor.ImplementationFactory is not null)
            return descriptor.ImplementationFactory(services);

        if (descriptor.ImplementationType is null)
            throw new InvalidOperationException("Cannot instantiate service, no implementation or factory specified.");

        return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType);
    }

    /// <summary>
    ///		Apply the factory pattern to the Dependency Injection container.
    /// </summary>
    /// <see href="https://espressocoder.com/2018/10/08/injecting-a-factory-service-in-asp-net-core/"/>
    /// <typeparam name="TService">The type of the service to generate a factory for</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the factory in</param>
    /// <remarks>
    ///		This method adds a <c><see cref="Func{TResult}"/>&lt;<typeparamref name="TService"/>&gt;</c> service to the
    ///		<paramref name="services"/> collection that can be used as a factory in any dependent services.
    /// </remarks>
    public static void AddFactory<TService>(this IServiceCollection services)
        where TService : class
    {
        services.AddTransient<Func<TService>>(x => () => x.GetRequiredService<TService>());
    }

    /// <summary>
    ///		Apply the type factory pattern to the Dependency Injection container
    /// </summary>
    /// <typeparam name="TService">The type of the service to generate a factory for</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the factory in</param>
    /// <remarks>
    /// This method adds a <c><see cref="Func{TResult}"/>&lt;<typeparamref name="TService"/>&gt;</c> service to the
    /// <paramref name="services"/> collection that can be used as a factory in any dependent services.
    /// </remarks>
    public static void AddTypeStringFactory<TService>(this IServiceCollection services)
        where TService : class
    {
        services.AddTransient<ITypeStringFactory<TService>, TypeStringFactory<TService>>();
    }
}