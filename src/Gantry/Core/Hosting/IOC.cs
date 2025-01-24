using ApacheTech.Common.BrighterSlim;

namespace Gantry.Core.Hosting;

/// <summary>
///     Globally accessible services, populated through the IOC Container. If a derived
///     instance of type <see cref="ModHost"/> has not been created within the
///     Application layer, these services will not be available, and will have to be
///     instantiated manually.<br/><br/>
///
///     Globally static objects like this are required because no <see cref="ModSystem"/>
///     can make use of dependency injection directly, because they are instantiated by
///     the game. Instead of using traditional singleton instances, where we'd need to
///     add boilerplate every time it's needed, we can cache the instances here, and then
///     used them anywhere they are needed, in lieu of being able to pass it in through
///     an importing constructor.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class IOC
{
    /// <summary>
    ///     Gets the IOC Resolver for the Server.
    /// </summary>
    /// <value>The IOC Resolver for the Server.</value>
    internal static IServiceProvider ServerIOC { get; set; }

    /// <summary>
    ///     Gets the IOC Resolver for the Client.
    /// </summary>
    /// <value>The IOC Resolver for the Client.</value>
    internal static IServiceProvider ClientIOC { get; set; }

    /// <summary>
    ///     Gets the IOC Resolver for the current app-side.
    /// </summary>      
    /// <value>The IOC Resolver for the current app-side.</value>
    public static IServiceProvider Services => ApiEx.OneOf(ClientIOC, ServerIOC);
    
    /// <summary>
    ///     Universal access to the Brighter command processor.
    /// </summary>
    public static IAmACommandProcessor Brighter => Services.Resolve<IAmACommandProcessor>();
}