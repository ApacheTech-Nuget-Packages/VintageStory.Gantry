using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Gantry.Services.IPC;

/// <summary>
///     
/// </summary>
public class MessageListener
{
    private readonly int _port;
    private readonly Action<string> _messageHandler;
    private readonly Thread _listenerThread;
    private volatile bool _isListening;

    /// <summary>
    ///     Initialises a new instance of the <see cref="MessageListener"/> class.
    /// </summary>
    /// <param name="port">The port to listen for incoming messages.</param>
    /// <param name="messageHandler">The action to invoke when a message is received.</param>
    public MessageListener(int port, Action<string> messageHandler)
    {
        _port = port;
        _messageHandler = messageHandler;
        _listenerThread = new Thread(ListenForMessages);
    }

    /// <summary>
    ///     Checks if the specified port is available without using exception flow control.
    /// </summary>
    /// <param name="port">The port to check for availability.</param>
    /// <returns>True if the port is available, otherwise false.</returns>
    public static bool IsPortAvailable(int port)
        => IPGlobalProperties
            .GetIPGlobalProperties()
            .GetActiveTcpConnections()
            .All(p => p.LocalEndPoint.Port != port);

    /// <summary>
    ///     Starts the listener thread to begin listening for incoming messages.
    /// </summary>
    public void Start()
    {
        _isListening = true;
        _listenerThread.Start();
    }

    /// <summary>
    ///     Stops the listener from accepting messages.
    /// </summary>
    public void Stop()
    {
        _isListening = false;
        _listenerThread.Join();
    }

    private void ListenForMessages()
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();

        while (_isListening)
        {
            try
            {
                var client = listener.AcceptTcpClient();
                var stream = client.GetStream();
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    _messageHandler?.Invoke(message);
                }

                client.Close();
            }
            catch (Exception ex)
            {
                G.Logger.Error($"IPC Listener Error (localhost:{_port}): {ex.Message}");
            }
        }
    }
}