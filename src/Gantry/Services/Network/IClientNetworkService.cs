namespace Gantry.Services.Network;

/// <summary>
///     Provides narrowed scope access to network channels within the game.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IClientNetworkService
{
    /// <summary>
    ///     Retrieves a client-side network channel.
    /// </summary>
    /// <param name="channelName">Name of the channel.</param>
    /// <returns>An instance of <see cref="IClientNetworkChannel"/>, used to send and receive network messages on the client.</returns>
    IClientNetworkChannel ClientChannel(string channelName);

    /// <summary>
    ///     Retrieves the mod's default client-side network channel.
    /// </summary>
    /// <returns>An instance of <see cref="IClientNetworkChannel"/>, used to send and receive network messages on the client.</returns>
    IClientNetworkChannel DefaultClientChannel { get; }

    /// <summary>
    ///     Registers a network channel on the client.
    /// </summary>
    /// <param name="channelName">The name of the channel to register.</param>
    void RegisterClientChannel(string channelName);
}