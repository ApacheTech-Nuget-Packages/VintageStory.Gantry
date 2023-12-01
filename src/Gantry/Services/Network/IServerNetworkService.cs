using JetBrains.Annotations;
using Vintagestory.API.Server;

namespace Gantry.Services.Network;

/// <summary>
///     Provides narrowed scope access to network channels within the game.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IServerNetworkService
{
    /// <summary>
    ///     Retrieves a server-side network channel.
    /// </summary>
    /// <param name="channelName">Name of the channel.</param>
    /// <returns>An instance of <see cref="IServerNetworkChannel"/>, used to send and receive network messages on the server.</returns>
    IServerNetworkChannel ServerChannel(string channelName);

    /// <summary>
    ///     Retrieves the mod's default server-side network channel.
    /// </summary>
    /// <returns>An instance of <see cref="IServerNetworkChannel"/>, used to send and receive network messages on the server.</returns>
    IServerNetworkChannel DefaultServerChannel { get; }

    /// <summary>
    ///     Registers a network channel on the server.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    void RegisterServerChannel(string channelName);
}