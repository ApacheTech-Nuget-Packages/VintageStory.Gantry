using Gantry.Core.ModSystems.Abstractions;

namespace Gantry.Services.IPC;

/// <summary>
///     A subsystem that handles messaging for a specific side of the application (Client/Server).
/// </summary>
public abstract class MessagingSubsystem : GantrySubsystem
{
    private readonly MessageListener _messageListener;
    private readonly MessageSender _messageSender;
    private readonly EnumAppSide _side;

    /// <inheritdoc />
    public override bool Enabled => false;

    /// <summary>
    ///     The port the listener is listening to.
    /// </summary>
    protected int ListenerPort { get; private set; }

    /// <summary>
    ///     The port the sender sends to.
    /// </summary>
    protected int SenderPort { get; private set; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="MessagingSubsystem"/> class.
    /// </summary>
    /// <param name="side"></param>
    /// <param name="listener"></param>
    /// <param name="sender"></param>
    protected MessagingSubsystem(EnumAppSide side, int listener, int sender)
    {
        _side = side;
        _messageListener = new MessageListener(ListenerPort = listener, IncomingMessageHandler);
        _messageSender = new MessageSender("localhost", SenderPort = sender);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Determines whether this subsystem should be loaded for the specified side.
    /// </summary>
    public override bool ShouldLoad(EnumAppSide forSide) => forSide.Is(_side);

    /// <inheritdoc />
    /// <summary>
    ///     Starts the message listener and sender for the subsystem.
    /// </summary>
    public override void Start(ICoreAPI api)
    {
        ApiEx.Logger.Debug($"{GetType().Name} starting.");
        if (!MessageListener.IsPortAvailable(ListenerPort))
        {
            ApiEx.Logger.Error($"Could not start message listener. Port {ListenerPort} is in use.");
            return;
        }
        _messageListener.Start();
        ApiEx.Logger.Debug($"{GetType().Name} now listening on port {ListenerPort}");
        base.Start(api);
    }

    /// <summary>
    ///     Handles incoming messages for the messaging subsystem.
    /// </summary>
    /// <param name="message">The incoming message.</param>
    public abstract void IncomingMessageHandler(string message);
    
    /// <summary>
    ///     Sends a message using the messaging subsystem.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void SendMessage(string message)
        => _messageSender.SendMessage(message);
}

///// <summary>
/////     A client-side implementation of the <see cref="MessagingSubsystem"/> for handling messaging as a client.
///// </summary>
//public class ClientIpc() : MessagingSubsystem(EnumAppSide.Client, listener:5000, sender:5001)
//{
//    /// <inheritdoc />
//    public override void IncomingMessageHandler(string message)
//    {
//        ApiEx.Client.SendChatMessage(message);
//    }
//}

///// <summary>
/////     A server-side implementation of the <see cref="MessagingSubsystem"/> for handling messaging as a server.
///// </summary>
//public class ServerIpc() : MessagingSubsystem(EnumAppSide.Server, listener:5002, sender:5003)
//{
//    /// <inheritdoc />
//    public override void IncomingMessageHandler(string message)
//    {
//        ApiEx.Server.BroadcastMessageToAllGroups(message, EnumChatType.Notification);
//    }
//}
