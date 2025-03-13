using System.Runtime.CompilerServices;
using ApacheTech.Common.Extensions.Harmony;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Gantry.Services.Network.Extensions;

/// <summary>
///     Extension methods for networking between client and server.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class NetworkChannelExtensions
{
    /// <summary>
    ///     Gets or registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="channelName">The name of the channel to register.</param>
    public static IServerNetworkChannel GetOrRegisterChannel(this IServerNetworkAPI napi, string channelName)
        => napi.GetChannel(channelName) ?? napi.RegisterChannel(channelName);

    /// <summary>
    ///     Gets or registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="channelName">The name of the channel to register.</param>
    public static IClientNetworkChannel GetOrRegisterChannel(this IClientNetworkAPI napi, string channelName)
        => napi.GetChannel(channelName) ?? napi.RegisterChannel(channelName);

    /// <summary>
    ///     Unregisters a specific message handler from the <see cref="IClientNetworkChannel"/>.
    /// </summary>
    /// <typeparam name="T">The type of message to unregister.</typeparam>
    /// <param name="channel">The channel.</param>
    /// <returns>The same <see cref="IClientNetworkChannel"/> that made the request.</returns>
    /// <exception cref="KeyNotFoundException">No such message type {typeFromHandle} registered. Did you forgot to call RegisterMessageType?</exception>
    /// <exception cref="ArgumentException">Please do not use array messages, they seem to cause serialisation problems in rare cases. Pack that array into its own class.</exception>
    public static IClientNetworkChannel UnregisterMessageHandler<T>(this IClientNetworkChannel channel)
    {
        var concrete = (NetworkChannel)channel;
        var messageTypes = concrete.GetField<Dictionary<Type, int>>("messageTypes");
        var typeFromHandle = typeof(T);
        if (!messageTypes.TryGetValue(typeof(T), out var index))
        {
            throw new KeyNotFoundException($"No such message type {typeFromHandle} registered. Did you forgot to call RegisterMessageType?");
        }
        if (typeof(T).IsArray)
        {
            throw new ArgumentException("Please do not use array messages, they seem to cause serialisation problems in rare cases. Pack that array into its own class.");
        }
        var handlers = concrete.GetField<Action<object>[]>("handlers");
        handlers.RemoveAt(index);
        return channel;
    }

    /// <summary>
    ///     Unregisters a specific message handler from the <see cref="IServerNetworkChannel"/>.
    /// </summary>
    /// <typeparam name="T">The type of message to unregister.</typeparam>
    /// <param name="channel">The channel.</param>
    /// <returns>The same <see cref="IServerNetworkChannel"/> that made the request.</returns>
    /// <exception cref="KeyNotFoundException">No such message type {typeFromHandle} registered. Did you forgot to call RegisterMessageType?</exception>
    /// <exception cref="ArgumentException">Please do not use array messages, they seem to cause serialisation problems in rare cases. Pack that array into its own class.</exception>
    public static IServerNetworkChannel UnregisterMessageHandler<T>(this IServerNetworkChannel channel)
    {
        var concrete = (Vintagestory.Server.NetworkChannel)channel;
        var messageTypes = concrete.GetField<Dictionary<Type, int>>("messageTypes");
        var typeFromHandle = typeof(T);
        if (!messageTypes.TryGetValue(typeof(T), out var index))
        {
            throw new KeyNotFoundException($"No such message type {typeFromHandle} registered. Did you forgot to call RegisterMessageType?");
        }
        if (typeof(T).IsArray)
        {
            throw new ArgumentException("Please do not use array messages, they seem to cause serialisation problems in rare cases. Pack that array into its own class.");
        }
        var handlers = concrete.GetField<Action<object>[]>("handlers");
        handlers.RemoveAt(index);
        return channel;
    }

    /// <summary>
    ///     Registers a message handler for the specified message type on a client network channel.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="channel">The client network channel to register the handler with.</param>
    /// <param name="handler">The handler to invoke when the message is received.</param>
    /// <returns>The client network channel with the registered handler.</returns>
    public static IClientNetworkChannel RegisterMessageHandler<T>(this IClientNetworkChannel channel, NetworkServerMessageHandler<T> handler)
        => channel.RegisterMessageType<T>().SetMessageHandler(handler);

    /// <summary>
    ///     Registers a message handler for the specified message type on a server network channel.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="channel">The server network channel to register the handler with.</param>
    /// <param name="handler">The handler to invoke when the message is received.</param>
    /// <returns>The server network channel with the registered handler.</returns>
    public static IServerNetworkChannel RegisterMessageHandler<T>(this IServerNetworkChannel channel, NetworkClientMessageHandler<T> handler)
        => channel.RegisterMessageType<T>().SetMessageHandler(handler);
}