using ApacheTech.Common.Extensions.Harmony;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;

namespace Gantry.Services.Network.Extensions;

/// <summary>
///     Extension methods for networking between client and server.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class NetworkChannelExtensions
{
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
        handlers.RemoveEntry(index);
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
        handlers.RemoveEntry(index);
        return channel;
    }
}