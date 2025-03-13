namespace Gantry.Services.Network;

/// <summary>
///     Provides narrowed scope access to network channels within the game.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IClientNetworkService : IGantryNetworkService
{
    /// <summary>
    ///     Retrieves the mod's default client-side network channel.
    /// </summary>
    /// <returns>An instance of <see cref="IClientNetworkChannel"/>, used to send and receive network messages on the client.</returns>
    IClientNetworkChannel DefaultClientChannel { get; }

    /// <summary>
    ///     Gets or registers a network channel on the client.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    new IClientNetworkChannel GetOrRegisterChannel(string channelName)
        => (this as IGantryNetworkService).GetOrRegisterChannel(channelName).To<IClientNetworkChannel>();
}