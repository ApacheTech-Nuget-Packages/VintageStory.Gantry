using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.Abstractions;
using Gantry.Core.Hosting.Annotation;
using Gantry.Extensions.Api;

namespace Gantry.Core.Hosting.Extensions;

/// <summary>
///     Extension method that aid the registration and creation of services.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    ///     Determines whether a constructor is decorated with a <see cref="SidedConstructorAttribute"/> attribute that matched the current app-side.
    /// </summary>
    /// <param name="constructor">The constructor to check.</param>
    /// <param name="side">The app-side to check against.</param>
    /// <returns><c>true</c> if the dependencies for the constructor should be resolved via the service provider, <c>false</c> otherwise.</returns>
    public static bool IsSided(this ConstructorInfo constructor, EnumAppSide side)
    {
        return constructor
            .GetCustomAttributes(typeof(SidedConstructorAttribute), false)
            .Cast<SidedConstructorAttribute>()
            .Any(q => q.Side == EnumAppSide.Universal || q.Side == side);
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
        => ActivatorEx.CreateInstance(provider, serviceType, args);

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
        => CreateSidedInstance(provider, typeof(T), args).To<T>();

    /// <summary>
    ///     Registers all <see cref="ModSystem"/>s in the current mod, into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the <see cref="ModSystem"/>s to.</param>
    /// <param name="gantry">Access to the Core Gantry API.</param>
    public static void AddModSystems(this IServiceCollection services, ICoreGantryAPI gantry)
    {
        var modSystems = gantry.Uapi.ModLoader.Systems.Where(p =>
        {
            try
            {
                return p.ShouldLoad(gantry.Uapi.Side);
            }
            catch (Exception ex)
            {
                gantry.Logger.Error($"Could not add mod `{p.GetType().FullName}` to the service collection.");
                gantry.Logger.Error(ex);
                return false;
            }

        });

        foreach (var system in modSystems)
        {
            services.AddSingleton(system.GetType(), system);
            gantry.Logger.VerboseDebug($" - Mod System: {system.GetType().Name}");
        }
    }

    /// <summary>
    ///     Registers all <see cref="ClientSystem"/>s into the service collection on the client.
    ///     Registers all <see cref="ServerSystem"/>s into the service collection on the client.
    /// </summary>
    /// <param name="services">The service collection to add the systems to.</param>
    /// <param name="gantry">Provides access to the Core Gantry API.</param>
    public static void AddSystems(this IServiceCollection services, ICoreGantryAPI gantry)
    {
        var api = gantry.Uapi;

        api.Invoke(
            capi =>
            {
                var clientSystems = capi.AsClientMain().GetField<ClientSystem[]>("clientSystems");
                if (clientSystems is null) return;
                foreach (var system in clientSystems)
                {
                    services.AddSingleton(system.GetType(), system);
                    gantry.Logger.VerboseDebug($" - Client System: {system.GetType().Name}");
                }
            },
            sapi =>
            {
                var serverSystems = sapi.AsServerMain().GetField<ServerSystem[]>("Systems");
                if (serverSystems is null) return;
                foreach (var system in serverSystems)
                {
                    services.AddSingleton(system.GetType(), system);
                    gantry.Logger.VerboseDebug($" - Server System: {system.GetType().Name}");
                }
            });
    }

    /// <summary>
    ///     Registers a client side features into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the features to.</param>
    /// <param name="api">The api to load systems from.</param>
    public static void AddModSystem<T>(this IServiceCollection services, ICoreAPI api) where T : ModSystem
    {
        services.AddSingleton(_ => api.ModLoader.GetModSystem<T>());
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