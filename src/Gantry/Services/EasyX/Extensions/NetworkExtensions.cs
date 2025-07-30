namespace Gantry.Services.EasyX.Extensions;

/// <summary>
///     Extension methods to aid broadcasting unique packets to players.
/// </summary>
public static class NetworkExtensions
{
    /// <summary>
    ///     Broadcasts unique packets to players, using a factory method to build packets for each online client.
    /// </summary>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <param name="serverNetworkChannel">The server network channel.</param>
    /// <param name="serverMain">Provides access to the server's main API, including player information.</param>
    /// <param name="packetFactory">The packet factory.</param>
    /// <param name="skipPlayers">A list of players the packets should not be created for, or sent to.</param>
    public static void BroadcastUniquePacket<TPacket>(this IServerNetworkChannel serverNetworkChannel, ServerMain serverMain,
        System.Func<IPlayer, TPacket> packetFactory, params IServerPlayer[] skipPlayers)
    {
        var players = serverMain.PlayersByUid.Values.Except(skipPlayers);
        foreach (var player in players)
        {
            serverNetworkChannel.SendPacket(packetFactory(player), player);
        }
    }
}