namespace Gantry.Services.Network;

/// <summary>
///     A service that provides narrowed scope access to network channels within the game.
/// </summary>
public interface IGantryNetworkService
{
    /// <summary>
    ///     Gets or registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    INetworkChannel GetOrRegisterChannel(string channelName);
}