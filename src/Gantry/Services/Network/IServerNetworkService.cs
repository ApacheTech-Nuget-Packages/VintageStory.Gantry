using Vintagestory.API.Server;

namespace Gantry.Services.Network;

/// <summary>
///     Provides narrowed scope access to network channels within the game.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IServerNetworkService : IGantryNetworkService
{
    /// <summary>
    ///     Retrieves the mod's default server-side network channel.
    /// </summary>
    /// <returns>An instance of <see cref="IServerNetworkChannel"/>, used to send and receive network messages on the server.</returns>
    IServerNetworkChannel DefaultServerChannel { get; }

    /// <summary>
    ///     Gets or registers a network channel on the server.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    new IServerNetworkChannel GetOrRegisterChannel(string channelName) 
        => (this as IGantryNetworkService).GetOrRegisterChannel(channelName).To<IServerNetworkChannel>();
}