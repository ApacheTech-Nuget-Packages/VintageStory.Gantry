namespace Gantry.Services.Network;

/// <summary>
///     A service that provides narrowed scope access to network channels within the game.
/// </summary>
/// <seealso cref="IClientNetworkService" />
/// <seealso cref="IServerNetworkService" />
public interface IUniversalNetworkService : IClientNetworkService, IServerNetworkService
{
    /// <summary>
    ///     Registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    void RegisterChannel(string channelName);
}