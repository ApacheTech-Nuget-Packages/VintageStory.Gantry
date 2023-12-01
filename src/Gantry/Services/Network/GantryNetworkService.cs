using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
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
    private readonly ICoreClientAPI _capi;
    private readonly ICoreServerAPI _sapi;
    private readonly string _defaultChannelName;

    /// <summary>
    /// 	Initialises a new instance of the <see cref="GantryNetworkService"/> class.
    /// </summary>
    /// <param name="api">The universal Core API.</param>
    public GantryNetworkService(ICoreAPI api) : this(api, NetworkServiceOptions.Default)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="GantryNetworkService"/> class.
    /// </summary>
    /// <param name="api">The universal Core API.</param>
    /// <param name="options">The options to pass to the service.</param>
    public GantryNetworkService(ICoreAPI api, NetworkServiceOptions options)
    {
        _defaultChannelName = options.DefaultChannelName;
        api.SetSidedInstances(ref _capi, ref _sapi);
    }

    /// <summary>
    ///     Retrieves a client-side network channel.
    /// </summary>
    /// <param name="channelName">Name of the channel.</param>
    /// <returns>An instance of <see cref="IClientNetworkChannel" />, used to send and receive network messages on the client.</returns>
    public IClientNetworkChannel ClientChannel(string channelName)
    {
        var channel = _capi?.Network.GetChannel(channelName);
        return channel ?? _capi?.Network.RegisterChannel(channelName);
    }

    /// <summary>
    ///     Retrieves a server-side network channel.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    /// <returns>An instance of <see cref="IServerNetworkChannel" />, used to send and receive network messages on the server.</returns>
    public IServerNetworkChannel ServerChannel(string channelName)
    {
        var channel = _sapi?.Network.GetChannel(channelName);
        return channel ?? _sapi?.Network.RegisterChannel(channelName);
    }

    /// <summary>
    ///     Retrieves the mod's default server-side network channel.
    /// </summary>
    /// <value>The default server channel.</value>
    public IServerNetworkChannel DefaultServerChannel => ServerChannel(_defaultChannelName);

    /// <summary>
    ///     Retrieves the mod's default client-side network channel.
    /// </summary>
    /// <value>The default client channel.</value>
    public IClientNetworkChannel DefaultClientChannel => ClientChannel(_defaultChannelName);

    /// <summary>
    ///     Registers a network channel on the server.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    public void RegisterServerChannel(string channelName)
    {
        if (_sapi is null || _sapi.Network.GetChannel(channelName) is not null) return;
        _sapi.Network.RegisterChannel(channelName);
    }

    /// <summary>
    ///     Registers a network channel on client.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    public void RegisterClientChannel(string channelName)
    {
        if (_capi.Network.GetChannel(channelName) is not null) return;
        _capi.Network.RegisterChannel(channelName);
    }

    /// <summary>
    ///     Registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    public void RegisterChannel(string channelName)
    {
        ApiEx.Run(RegisterClientChannel, RegisterServerChannel, channelName);
    }
}