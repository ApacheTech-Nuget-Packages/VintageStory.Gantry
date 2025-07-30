namespace Gantry.Extensions.Api;

/// <summary>
///     Provides extension methods for working with the Core API, enabling side-dependent logic for client and server execution contexts.
/// </summary>
public static class AppSideExtensions
{
    /// <summary>
    ///     Chooses between one of two objects, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientObject">The object to return if called from the client.</param>
    /// <param name="serverObject">The object to return if called from the server.</param>
    /// <returns>
    ///     Returns <paramref name="clientObject"/> if called from the client, or <paramref name="serverObject"/> if called from the server.
    /// </returns>
    public static T OneOf<T>(this EnumAppSide side, T clientObject, T serverObject) 
        => side.IsClient() ? clientObject : serverObject;

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The action to invoke if on the client.</param>
    /// <param name="serverAction">The action to invoke if on the server.</param>
    public static void Invoke(this EnumAppSide side, Action clientAction, Action serverAction) 
        => side.OneOf(clientAction, serverAction).Invoke();

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server, using the API instance.
    /// </summary>
    /// <param name="api">The API instance used to determine the execution side.</param>
    /// <param name="clientAction">The action to invoke if on the client, accepting a client API instance.</param>
    /// <param name="serverAction">The action to invoke if on the server, accepting a server API instance.</param>
    public static void Invoke(this ICoreAPI api, Action<ICoreClientAPI> clientAction, Action<ICoreServerAPI> serverAction)
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
    ///     Invokes a side-dependent action with a parameter, based on whether it's being called by the client or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <typeparam name="T">The type of the parameter to pass to the action.</typeparam>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The action to invoke if on the client.</param>
    /// <param name="serverAction">The action to invoke if on the server.</param>
    /// <param name="parameter">The parameter to pass to the invoked action.</param>
    public static void Invoke<T>(this EnumAppSide side, Action<T> clientAction, Action<T> serverAction, T parameter) 
        => side.OneOf(clientAction, serverAction).Invoke(parameter);

    /// <summary>
    ///     Invokes a side-dependent function and returns its result, based on whether it's being called by the client or the server.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The function to invoke if on the client.</param>
    /// <param name="serverAction">The function to invoke if on the server.</param>
    /// <returns>The result of the invoked function.</returns>
    public static T? Invoke<T>(this EnumAppSide side, Func<T> clientAction, Func<T> serverAction) 
        => (T?)side.OneOf(clientAction, serverAction).DynamicInvoke();

    /// <summary>
    ///     Executes a side-dependent function based on whether the API instance is client or server, and returns its result.
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
    public static T Invoke<T>(this ICoreAPI api, System.Func<ICoreClientAPI, T> cf, System.Func<ICoreServerAPI, T> sf) => api.Side switch
    {
        EnumAppSide.Server => sf((ICoreServerAPI)api),
        EnumAppSide.Client => cf((ICoreClientAPI)api),
        _ => throw new InvalidOperationException("Cannot invoke side-dependent function on universal API."),
    };

    /// <summary>
    ///     Invokes a side-dependent function with a parameter and returns its result, based on whether it's being called by the client or the server.
    /// </summary>
    /// <typeparam name="T">The return type of the function and the type of the parameter.</typeparam>
    /// <param name="side">The app side in question.</param>
    /// <param name="clientAction">The function to invoke if on the client.</param>
    /// <param name="serverAction">The function to invoke if on the server.</param>
    /// <param name="parameter">The parameter to pass to the invoked function.</param>
    /// <returns>The result of the invoked function.</returns>
    public static T? Invoke<T>(this EnumAppSide side, Func<T> clientAction, Func<T> serverAction, T parameter)
        => (T?)side.OneOf(clientAction, serverAction).DynamicInvoke(parameter);

    /// <summary>
    ///     Populates client, or server specific API members, dependent on which app side the method is called from.
    /// </summary>
    /// <param name="api">The universal API to cast from.</param>
    /// <param name="clientApi">The client API member to populate, if on the Client app side.</param>
    /// <param name="serverApi">The server API member to populate, if on the Server app side.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the API side is not recognised.</exception>
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