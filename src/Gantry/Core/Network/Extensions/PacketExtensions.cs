namespace Gantry.Core.Network.Extensions;

/// <summary>
///     Extension methods to aid working with network packets.
/// </summary>
public static class PacketExtensions
{
    /// <summary>
    ///     Sends a packet to the server.
    /// </summary>
    /// <typeparam name="T">The type of packet to send to the server.</typeparam>
    /// <param name="channel">The channel on which to send the packet.</param>
    public static void SendPacket<T>(this IClientNetworkChannel channel) where T : new() 
        => channel.SendPacket(new T());

    /// <summary>
    ///     Sends a packet to a set list of players.
    /// </summary>
    /// <typeparam name="T">The type of packet to send to the server.</typeparam>
    /// <param name="channel">The channel on which to send the packet.</param>
    public static void SendPacket<T>(this IServerNetworkChannel channel) where T : new() 
        => channel.SendPacket(new T());

    /// <summary>
    ///     Sends a packet to a set list of players.
    /// </summary>
    /// <typeparam name="T">The type of packet to send to the server.</typeparam>
    /// <param name="channel">The channel on which to send the packet.</param>
    /// <param name="players">The players to send the packet to.</param>
    public static void SendPacket<T>(this IServerNetworkChannel channel, params IServerPlayer[] players) where T : new() 
        => channel.SendPacket(new T(), players);

    /// <summary>
    ///     Broadcasts a packet to the server.
    /// </summary>
    /// <typeparam name="T">The type of packet to send to the server.</typeparam>
    /// <param name="channel">The channel on which to send the packet.</param>
    public static void BroadcastPacket<T>(this IServerNetworkChannel channel) where T : new() 
        => channel.BroadcastPacket(new T());

    /// <summary>
    ///     Broadcasts a packet to the server.
    /// </summary>
    /// <typeparam name="T">The type of packet to send to the server.</typeparam>
    /// <param name="channel">The channel on which to send the packet.</param>
    /// <param name="exceptPlayers">The players to not send the packet to.</param>
    public static void BroadcastPacket<T>(this IServerNetworkChannel channel, params IServerPlayer[] exceptPlayers) where T : new() 
        => channel.BroadcastPacket(new T(), exceptPlayers);
}