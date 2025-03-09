using Gantry.Core.Annotation;
using Vintagestory.API.Server;

namespace Gantry.Services.Network;

/// <summary>
///     A service that provides narrowed scope access to network channels within the game.
/// </summary>
/// <seealso cref="IClientNetworkService" />
/// <seealso cref="IServerNetworkService" />
/// <seealso cref="IUniversalNetworkService" />
public class GantryNetworkService : IUniversalNetworkService
{
    private readonly NetworkServiceOptions _options;

    /// <summary>
    /// 	Initialises a new instance of the <see cref="GantryNetworkService"/> class.
    /// </summary>
    [Universal]
    public GantryNetworkService() : this(NetworkServiceOptions.Default)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="GantryNetworkService"/> class.
    /// </summary>
    /// <param name="options">The options to pass to the service.</param>
    [Universal]
    public GantryNetworkService(NetworkServiceOptions options)
    {
        _options = options;
    }

    /// <summary>
    ///     Retrieves a client-side network channel.
    /// </summary>
    /// <param name="channelName">Name of the channel.</param>
    /// <returns>An instance of <see cref="IClientNetworkChannel" />, used to send and receive network messages on the client.</returns>
    [ClientSide]
    public IClientNetworkChannel ClientChannel(string channelName)
    {
        var capi = ApiEx.Client;
        RegisterClientChannel(channelName);
        var channel = capi.Network.GetChannel(channelName);
        return channel;
    }

    /// <summary>
    ///     Retrieves a server-side network channel.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    /// <returns>An instance of <see cref="IServerNetworkChannel" />, used to send and receive network messages on the server.</returns>
    [ServerSide]
    public IServerNetworkChannel ServerChannel(string channelName)
    {
        var sapi = ApiEx.Server;
        RegisterServerChannel(channelName);
        var channel = sapi.Network.GetChannel(channelName);
        return channel;
    }

    /// <summary>
    ///     Retrieves the mod's default server-side network channel.
    /// </summary>
    /// <value>The default server channel.</value>
    [ServerSide]
    public IServerNetworkChannel DefaultServerChannel => ServerChannel(_options.DefaultChannelName);

    /// <summary>
    ///     Retrieves the mod's default client-side network channel.
    /// </summary>
    /// <value>The default client channel.</value>
    [ClientSide]
    public IClientNetworkChannel DefaultClientChannel => ClientChannel(_options.DefaultChannelName);

    /// <summary>
    ///     Registers a network channel on the server.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    [ServerSide]
    public void RegisterServerChannel(string channelName)
    {
        var sapi = ApiEx.Server;
        if (sapi.Network.GetChannel(channelName) is not null) return;
        ApiEx.Logger.VerboseDebug($"Registering server network channel: {channelName}");
        sapi.Network.RegisterChannel(channelName);
    }

    /// <summary>
    ///     Registers a network channel on client.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    [ClientSide]
    public void RegisterClientChannel(string channelName)
    {
        var capi = ApiEx.Client;
        if (capi.Network.GetChannel(channelName) is not null) return;
        ApiEx.Logger.VerboseDebug($"Registering client network channel: {channelName}");
        capi.Network.RegisterChannel(channelName);
    }

    /// <summary>
    ///     Registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    [Universal]
    public void RegisterChannel(string channelName)
    {
        ApiEx.Run(RegisterClientChannel, RegisterServerChannel, channelName);
    }
}