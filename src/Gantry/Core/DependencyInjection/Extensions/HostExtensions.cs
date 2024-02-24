#nullable enable
using System.Reflection;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.DependencyInjection.Annotation;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core.DependencyInjection.Extensions;

/// <summary>
///     Extension method that aid the registration and creation of services.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class HostExtensions
{
    /// <summary>
    ///     Determines whether a constructor is decorated with a <see cref="SidedConstructorAttribute"/> attribute that matched the current app-side.
    /// </summary>
    /// <param name="constructor">The constructor to check.</param>
    /// <returns><c>true</c> if the dependencies for the constructor should be resolved via the service provider, <c>false</c> otherwise.</returns>
    public static bool IOCEnabled(this ConstructorInfo constructor)
    {
        return constructor
            .GetCustomAttributes(typeof(SidedConstructorAttribute), false)
            .Cast<SidedConstructorAttribute>()
            .Any(q => q.Side == EnumAppSide.Universal || q.Side == ApiEx.Side);
    }

    /// <summary>
    ///     Creates an object of a specified type, using the IOC Container to resolve dependencies.
    /// </summary>
    /// <param name="provider">The service provider to use to resolve dependencies for the instantiated class.</param>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <param name="args">An optional list of arguments, sent the constructor of the instantiated class.</param>
    /// <returns>A service object of type <paramref name="serviceType" />.
    /// 
    /// -or-
    /// 
    /// <see langword="null" /> if no object of type <paramref name="serviceType" /> can be instantiated from the service collection.</returns>
    public static object CreateSidedInstance(this IServiceProvider provider, Type serviceType, params object[] args)
    {
        return ActivatorEx.CreateInstance(provider, serviceType, args);
    }

    /// <summary>
    ///     Creates an object of a specified type, using the IOC Container to resolve dependencies.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    /// <param name="provider">The service provider to use to resolve dependencies for the instantiated class.</param>
    /// <param name="args">An optional list of arguments, sent the constructor of the instantiated class.</param>
    /// <returns>An object of type <typeparamref name="T" />.
    /// 
    /// -or-
    /// 
    /// <see langword="null" /> if there is no object of type <typeparamref name="T" /> can be instantiated from the service collection.</returns>
    public static T CreateSidedInstance<T>(this IServiceProvider provider, params object[] args) where T : class
    {
        return (T)CreateSidedInstance(provider, typeof(T), args);
    }

    /// <summary>
    ///     Registers all <see cref="ModSystem"/>s in the current mod, into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the <see cref="ModSystem"/>s to.</param>
    /// <param name="side">The app side to load systems from.</param>
    public static void AddModSystems(this IServiceCollection services, EnumAppSide side)
    {
        var modSystems = ModEx.Mod.Systems.Where(p => p.ShouldLoad(side));
        foreach (var system in modSystems)
        {
            services.AddSingleton(system.GetType(), system);
        }
    }

    /// <summary>
    ///     Registers a client side features into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the features to.</param>
    public static void AddModSystem<T>(this IServiceCollection services) where T : ModSystem
    {
        services.AddSingleton(_ => ApiEx.Current.ModLoader.GetModSystem<T>());
    }

    /// <summary>
    ///     Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceProvider">And object that provides access to the service collection.</param>
    /// <param name="service">The service being requested.</param>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <returns>A service object of type <typeparamref name="T" />.
    /// 
    /// -or-
    /// 
    /// <see langword="null" /> if there is no service object of type <typeparamref name="T" />.</returns>
    public static bool TryResolve<T>(this IServiceProvider serviceProvider, out T? service) where T : class
    {
        try
        {
            service = serviceProvider.GetService(typeof(T)) as T;
        }
        catch (KeyNotFoundException)
        {
            service = null;
        }
        return service is not null;
    }

    /// <summary>
    ///     Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceProvider">And object that provides access to the service collection.</param>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <returns>A service object of type <typeparamref name="T" />.
    /// 
    /// -or-
    /// 
    /// <see langword="null" /> if there is no service object of type <typeparamref name="T" />.</returns>
    public static T? GetService<T>(this IServiceProvider serviceProvider) where T : class
        => serviceProvider.GetService(typeof(T)) as T;
}