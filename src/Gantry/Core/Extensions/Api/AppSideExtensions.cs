using Vintagestory.API.Server;

namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Extension method for working with the Core API.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class AppSideExtensions
{
    /// <summary>
    ///     Chooses between one of two objects, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientObject">The client object.</param>
    /// <param name="serverObject">The server object.</param>
    /// <returns>
    ///     Returns <paramref name="clientObject"/> if called from the client, or <paramref name="serverObject"/> if called from the server.
    /// </returns>
    public static T ChooseOneOf<T>(this EnumAppSide side, T clientObject, T serverObject)
    {
        return side.IsClient() ? clientObject : serverObject;
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static void RunOneOf(this EnumAppSide side, Action clientAction, Action serverAction)
    {
        side.ChooseOneOf(clientAction, serverAction).Invoke();
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="api">The app side in question.</param>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static void RunOneOf(this ICoreAPI api, Action<ICoreClientAPI> clientAction, Action<ICoreServerAPI> serverAction)
    {
        switch (api.Side)
        {
            case EnumAppSide.Server:
                serverAction((ICoreServerAPI)api);
                break;
            case EnumAppSide.Client:
                clientAction((ICoreClientAPI)api);
                break;
            case EnumAppSide.Universal:
                throw new InvalidOperationException("Cannot determine app-side. Enum evaluated to 'Universal'.");
            default:
                throw new ArgumentOutOfRangeException(nameof(clientAction));
        }
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    /// <param name="parameter">The parameter to pass to the invoked action.</param>
    public static void RunOneOf<T>(this EnumAppSide side, Action<T> clientAction, Action<T> serverAction, T parameter)
    {
        side.ChooseOneOf(clientAction, serverAction).Invoke(parameter);
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static T? ReturnOneOf<T>(this EnumAppSide side, Func<T> clientAction, Func<T> serverAction)
    {
        return (T?)side.ChooseOneOf(clientAction, serverAction).DynamicInvoke();
    }

    /// <summary>
    ///     Executes a side-dependent function based on whether the API instance is client or server.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="api">The API instance used to determine the execution side.</param>
    /// <param name="cf">The function to execute when on the client side.</param>
    /// <param name="sf">The function to execute when on the server side.</param>
    /// <returns>The result of the executed function.</returns>
    /// <remarks>
    ///     This method ensures that the appropriate function is invoked based on the execution context.
    ///     If the API is neither client nor server, an exception is thrown.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the API is not specifically client or server.
    /// </exception>
    public static T Invoke<T>(this ICoreAPI api, System.Func<ICoreClientAPI, T> cf, System.Func<ICoreServerAPI, T> sf)
    {
        return api.Side switch
        {
            EnumAppSide.Server => sf((ICoreServerAPI)api),
            EnumAppSide.Client => cf((ICoreClientAPI)api),
            _ => throw new InvalidOperationException("Cannot invoke side-dependent function on universal API."),
        };
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    /// <param name="parameter">The parameter to pass to the invoked action.</param>
    public static T? ReturnOneOf<T>(this EnumAppSide side, Func<T> clientAction, Func<T> serverAction, T parameter)
    {
        return (T?)side.ChooseOneOf(clientAction, serverAction).DynamicInvoke(parameter);
    }

    /// <summary>
    ///     Populates client, or server specific API members, dependent on which app side the method is called from.
    /// </summary>
    /// <param name="api">The universal API to cast from.</param>
    /// <param name="clientApi">The client API member to populate, if on the Client app side.</param>
    /// <param name="serverApi">The server API member to populate, if on the Server app side.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void SetSidedInstances(this ICoreAPI api, ref ICoreClientAPI clientApi, ref ICoreServerAPI serverApi)
    {
        switch (api.Side)
        {
            case EnumAppSide.Server:
                serverApi = (ICoreServerAPI)api;
                break;
            case EnumAppSide.Client:
                clientApi = (ICoreClientAPI)api;
                break;
            case EnumAppSide.Universal:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api));
        }
    }
}