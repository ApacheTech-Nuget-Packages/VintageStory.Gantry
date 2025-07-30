using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.Abstractions;
using Vintagestory.API.Util;

namespace Gantry.Core.Network.Extensions;

/// <summary>
///     Extension methods for networking between client and server.
/// </summary>
public static class NetworkChannelExtensions
{
    /// <summary>
    ///     Gets or registers the default server channel for the mod.
    /// </summary>
    /// <param name="napi">The game's server network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The default server network channel for the mod.
    ///     <para>
    ///         If the channel is already registered, it is returned; otherwise, a new channel is registered and returned.
    ///     </para>
    /// </returns>
    public static IServerNetworkChannel GetOrRegisterDefaultChannel(this IServerNetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Attempting to retrieve default server channel for mod '{modId}'.");
        var channel = napi.GetChannel(modId);
        if (channel is not null)
        {
            gapi.Log($"Existing default server channel found for mod '{modId}'.");
            return channel;
        }

        gapi.Log($"Default server channel not found for mod '{modId}'. Registering new channel.");
        channel = napi.RegisterChannel(modId);
        gapi.Log($"New default server channel registered for mod '{modId}'.");
        return channel;
    }

    /// <summary>
    ///     Gets the default server channel for the mod.
    /// </summary>
    /// <param name="napi">The game's server network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The default server network channel for the mod.
    ///     <para>
    ///         This may return null if the channel has not been registered.
    ///     </para>
    /// </returns>
    public static IServerNetworkChannel? GetDefaultChannel(this IServerNetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Retrieving default server channel for mod '{modId}'.");
        var channel = napi.GetChannel(modId);
        if (channel is not null)
        {
            gapi.Log($"Default server channel exists for mod '{modId}'.");
        }
        else
        {
            gapi.Log($"No default server channel registered for mod '{modId}'.");
        }
        return channel;
    }

    /// <summary>
    ///     Registers the default server channel for the mod.
    /// </summary>
    /// <param name="napi">The game's server network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The newly registered default server network channel for the mod.
    /// </returns>
    public static IServerNetworkChannel RegisterDefaultChannel(this IServerNetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Registering default server channel for mod '{modId}'.");
        var channel = napi.RegisterChannel(modId);
        gapi.Log($"Default server channel registered for mod '{modId}'.");
        return channel;
    }

    /// <summary>
    ///     Gets or registers the default client channel for the mod.
    /// </summary>
    /// <param name="napi">The game's client network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The default client network channel for the mod.
    ///     <para>
    ///         If the channel is already registered, it is returned; otherwise, a new channel is registered and returned.
    ///     </para>
    /// </returns>
    public static IClientNetworkChannel GetOrRegisterDefaultChannel(this IClientNetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Attempting to retrieve default client channel for mod '{modId}'.");
        var channel = napi.GetChannel(modId);
        if (channel is not null)
        {
            gapi.Log($"Existing default client channel found for mod '{modId}'.");
            return channel;
        }

        gapi.Log($"Default client channel not found for mod '{modId}'. Registering new channel.");
        channel = napi.RegisterChannel(modId);
        gapi.Log($"New default client channel registered for mod '{modId}'.");
        return channel;
    }

    /// <summary>
    ///     Gets the default client channel for the mod.
    /// </summary>
    /// <param name="napi">The game's client network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The default client network channel for the mod.
    ///     <para>
    ///         This may return null if the channel has not been registered.
    ///     </para>
    /// </returns>
    public static IClientNetworkChannel? GetDefaultChannel(this IClientNetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Retrieving default client channel for mod '{modId}'.");
        var channel = napi.GetChannel(modId);
        if (channel is not null)
        {
            gapi.Log($"Default client channel exists for mod '{modId}'.");
        }
        else
        {
            gapi.Logger.Error($"No default client channel registered for mod '{modId}'.");
        }
        return channel;
    }

    /// <summary>
    ///     Registers the default client channel for the mod.
    /// </summary>
    /// <param name="napi">The game's client network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The newly registered default client network channel for the mod.
    /// </returns>
    public static IClientNetworkChannel RegisterDefaultChannel(this IClientNetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Registering default client channel for mod '{modId}'.");
        var channel = napi.RegisterChannel(modId);
        gapi.Log($"Default client channel registered for mod '{modId}'.");
        return channel;
    }

    /// <summary>
    ///     Gets or registers the default network channel for the mod.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The default network channel for the mod.
    ///     <para>
    ///         If the channel is already registered, it is returned; otherwise, a new channel is registered and returned.
    ///     </para>
    /// </returns>
    public static INetworkChannel GetOrRegisterDefaultChannel(this INetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Attempting to retrieve default network channel for mod '{modId}'.");
        var channel = napi.GetChannel(modId);
        if (channel is not null)
        {
            gapi.Log($"Existing default network channel found for mod '{modId}'.");
            return channel;
        }

        gapi.Log($"Default network channel not found for mod '{modId}'. Registering new channel.");
        channel = napi.RegisterChannel(modId);
        gapi.Log($"Default network channel registered for mod '{modId}'.");
        return channel;
    }

    /// <summary>
    ///     Gets the default network channel for the mod.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The default network channel for the mod.
    ///     <para>
    ///         This may return null if the channel has not been registered.
    ///     </para>
    /// </returns>
    public static INetworkChannel? GetDefaultChannel(this INetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Retrieving default network channel for mod '{modId}'.");
        var channel = napi.GetChannel(modId);
        if (channel is not null)
        {
            gapi.Log($"Default network channel exists for mod '{modId}'.");
        }
        else
        {
            gapi.Log($"No default network channel registered for mod '{modId}'.");
        }
        return channel;
    }

    /// <summary>
    ///     Registers the default network channel for the mod.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The newly registered default network channel for the mod.
    /// </returns>
    public static INetworkChannel RegisterDefaultChannel(this INetworkAPI napi, ICoreGantryAPI gapi)
    {
        var modId = gapi.Mod.Info.ModID;
        gapi.Log($"Registering default network channel for mod '{modId}'.");
        var channel = napi.RegisterChannel(modId);
        gapi.Log($"Default network channel registered for mod '{modId}'.");
        return channel;
    }

    /// <summary>
    ///     Gets or registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <param name="channelName">The name of the channel to register.</param>
    /// <returns>
    ///     The server network channel corresponding to the provided channel name.
    /// </returns>
    public static IServerNetworkChannel GetOrRegisterChannel(this IServerNetworkAPI napi, ICoreGantryAPI gapi, string channelName)
    {
        gapi.Log($"Attempting to retrieve server channel '{channelName}'.");
        var channel = napi.GetChannel(channelName);
        if (channel is not null)
        {
            gapi.Log($"Server channel '{channelName}' retrieved successfully.");
            return channel;
        }

        gapi.Log($"Server channel '{channelName}' not found. Registering new server channel.");
        channel = napi.RegisterChannel(channelName);
        gapi.Log($"Server channel '{channelName}' registered successfully.");
        return channel;
    }

    /// <summary>
    ///     Gets or registers a network channel on the app side this method is called from.
    /// </summary>
    /// <param name="napi">The game's network API.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <param name="channelName">The name of the channel to register.</param>
    /// <returns>
    ///     The client network channel corresponding to the provided channel name.
    /// </returns>
    public static IClientNetworkChannel GetOrRegisterChannel(this IClientNetworkAPI napi, ICoreGantryAPI gapi, string channelName)
    {
        gapi.Log($"Attempting to retrieve client channel '{channelName}'.");
        var channel = napi.GetChannel(channelName);
        if (channel is not null)
        {
            gapi.Log($"Client channel '{channelName}' retrieved successfully.");
            return channel;
        }

        gapi.Log($"Client channel '{channelName}' not found. Registering new client channel.");
        channel = napi.RegisterChannel(channelName);
        gapi.Log($"Client channel '{channelName}' registered successfully.");
        return channel;
    }

    /// <summary>
    ///     Unregisters a specific message handler from the <see cref="IClientNetworkChannel"/>.
    /// </summary>
    /// <typeparam name="T">The type of message to unregister.</typeparam>
    /// <param name="channel">The channel.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The same <see cref="IClientNetworkChannel"/> that made the request.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     No such message type registered for type '{typeof(T)}'. Did you forget to call RegisterMessageType?
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Array messages are not allowed. Please pack that array into its own class.
    /// </exception>
    public static IClientNetworkChannel UnregisterMessageHandler<T>(this IClientNetworkChannel channel, ICoreGantryAPI gapi)
    {
        gapi.Log($"Attempting to unregister message handler for type '{typeof(T)}' on client channel.");
        var concrete = (Vintagestory.Client.NoObf.NetworkChannel)channel;
        var messageTypes = concrete.GetField<Dictionary<Type, int>>("messageTypes");
        var typeFromHandle = typeof(T);
        if (!messageTypes.TryGetValue(typeFromHandle, out var index))
        {
            gapi.Log($"Message type '{typeFromHandle}' not found on client channel.");
            throw new KeyNotFoundException($"No such message type {typeFromHandle} registered. Did you forget to call RegisterMessageType?");
        }
        if (typeFromHandle.IsArray)
        {
            gapi.Log($"Attempt to unregister array message type '{typeFromHandle}' on client channel.");
            throw new ArgumentException("Please do not use array messages, they seem to cause serialisation problems in rare cases. Pack that array into its own class.");
        }
        var handlers = concrete.GetField<Action<object>[]>("handlers");
        handlers.RemoveAt(index);
        gapi.Log($"Message handler for type '{typeFromHandle}' unregistered on client channel.");
        return channel;
    }

    /// <summary>
    ///     Unregisters a specific message handler from the <see cref="IServerNetworkChannel"/>.
    /// </summary>
    /// <typeparam name="T">The type of message to unregister.</typeparam>
    /// <param name="channel">The channel.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The same <see cref="IServerNetworkChannel"/> that made the request.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     No such message type registered for type '{typeof(T)}'. Did you forget to call RegisterMessageType?
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Array messages are not allowed. Please pack that array into its own class.
    /// </exception>
    public static IServerNetworkChannel UnregisterMessageHandler<T>(this IServerNetworkChannel channel, ICoreGantryAPI gapi)
    {
        gapi.Log($"Attempting to unregister message handler for type '{typeof(T)}' on server channel.");
        var concrete = (Vintagestory.Server.NetworkChannel)channel;
        var messageTypes = concrete.GetField<Dictionary<Type, int>>("messageTypes");
        var typeFromHandle = typeof(T);
        if (!messageTypes.TryGetValue(typeFromHandle, out var index))
        {
            gapi.Log($"Message type '{typeFromHandle}' not found on server channel.");
            throw new KeyNotFoundException($"No such message type {typeFromHandle} registered. Did you forget to call RegisterMessageType?");
        }
        if (typeFromHandle.IsArray)
        {
            gapi.Log($"Attempt to unregister array message type '{typeFromHandle}' on server channel.");
            throw new ArgumentException("Please do not use array messages, they seem to cause serialisation problems in rare cases. Pack that array into its own class.");
        }
        var handlers = concrete.GetField<Action<object>[]>("handlers");
        handlers.RemoveAt(index);
        gapi.Log($"Message handler for type '{typeFromHandle}' unregistered on server channel.");
        return channel;
    }

    /// <summary>
    ///     Registers a message handler for the specified message type on a client network channel.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="channel">The client network channel to register the handler with.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <param name="handler">The handler to invoke when the message is received.</param>
    /// <returns>
    ///     The client network channel with the registered handler.
    /// </returns>
    public static IClientNetworkChannel RegisterPacket<T>(this IClientNetworkChannel channel, ICoreGantryAPI gapi, NetworkServerMessageHandler<T> handler)
    {
        gapi.Log($"Registering duplex packet '{typeof(T)}' with handler on client channel.");
        return channel.RegisterMessageType<T>().SetMessageHandler(handler);
    }

    /// <summary>
    ///     Registers a message handler for the specified message type on a server network channel.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="channel">The server network channel to register the handler with.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <param name="handler">The handler to invoke when the message is received.</param>
    /// <returns>
    ///     The server network channel with the registered handler.
    /// </returns>
    public static IServerNetworkChannel RegisterPacket<T>(this IServerNetworkChannel channel, ICoreGantryAPI gapi, NetworkClientMessageHandler<T> handler)
    {
        gapi.Log($"Registering duplex packet '{typeof(T)}' with handler on server channel.");
        return channel.RegisterMessageType<T>().SetMessageHandler(handler);
    }

    /// <summary>
    ///     Registers a message handler for the specified message type on a server network channel.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="channel">The server network channel to register the handler with.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The server network channel with the registered handler.
    /// </returns>
    public static IClientNetworkChannel RegisterPacket<T>(this IClientNetworkChannel channel, ICoreGantryAPI gapi)
    {
        gapi.Log($"Registering simplex packet '{typeof(T)}' on client channel.");
        return channel.RegisterMessageType<T>();
    }

    /// <summary>
    ///     Registers a message handler for the specified message type on a server network channel.
    /// </summary>
    /// <typeparam name="T">The type of the message to handle.</typeparam>
    /// <param name="channel">The server network channel to register the handler with.</param>
    /// <param name="gapi">The game's gantry API.</param>
    /// <returns>
    ///     The server network channel with the registered handler.
    /// </returns>
    public static IServerNetworkChannel RegisterPacket<T>(this IServerNetworkChannel channel, ICoreGantryAPI gapi)
    {
        gapi.Log($"Registering simplex packet '{typeof(T)}' on server channel.");
        return channel.RegisterMessageType<T>();
    }
}