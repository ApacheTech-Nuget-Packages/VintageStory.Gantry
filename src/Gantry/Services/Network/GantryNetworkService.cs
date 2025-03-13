using Gantry.Core.Annotation;
using Vintagestory.API.Server;

namespace Gantry.Services.Network;

/// <summary>
///     A service that provides narrowed scope access to network channels within the game.
/// </summary>
/// <seealso cref="IClientNetworkService" />
/// <seealso cref="IServerNetworkService" />
/// <seealso cref="IGantryNetworkService" />
public class GantryNetworkService : IGantryNetworkService, IClientNetworkService, IServerNetworkService
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
    ///     Retrieves the mod's default server-side network channel.
    /// </summary>
    /// <value>The default server channel.</value>
    [ServerSide]
    public IServerNetworkChannel DefaultServerChannel 
        => GetOrRegisterChannel(_options.DefaultChannelName).To<IServerNetworkChannel>();

    /// <summary>
    ///     Retrieves the mod's default client-side network channel.
    /// </summary>
    /// <value>The default client channel.</value>
    [ClientSide]
    public IClientNetworkChannel DefaultClientChannel 
        => GetOrRegisterChannel(_options.DefaultChannelName).To<IClientNetworkChannel>();

    /// <summary>
    ///     Gets or registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    [Universal]
    public INetworkChannel GetOrRegisterChannel(string channelName)
    {
        try
        {
            var side = ApiEx.Side;
            var channel = ApiEx.Current.Network.GetChannel(channelName);
            if (channel is not null)
            {
                G.Log.VerboseDebug($"{side} network channel found: {channelName}");
            }
            else
            {
                G.Log.VerboseDebug($"{side} network channel {channelName} not found. Registering new channel.");
                channel = ApiEx.Current.Network.RegisterChannel(channelName);
                if (channel is null)
                {
                    G.Log.Error($"{side} network channel {channelName} not registered.");
                }
                else
                {
                    G.Log.VerboseDebug($"Registered {side} network channel: {channelName}");
                }
            }

            if (side.IsClient())
            {
                var state = ApiEx.Client.Network.GetChannelState(ModEx.ModInfo.ModID);
                G.Log.VerboseDebug($" - State: {state}");
                G.Log.VerboseDebug($" - Connected: {channel.To<IClientNetworkChannel>().Connected}");
            }

            return channel;
        }
        catch (Exception ex)
        {
            G.Log.Error($"Error while registering {ApiEx.Side} network channel: {channelName}");
            G.Log.Error(ex);
            throw;
        }
    }
}