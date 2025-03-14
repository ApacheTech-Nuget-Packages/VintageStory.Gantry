using System.Net.Sockets;
using System.Text;

namespace Gantry.Services.IPC;

/// <summary>
///     A class responsible for sending messages over TCP to a specified host and port.
/// </summary>
public class MessageSender(string host, int port)
{
    private readonly string _host = host;
    private readonly int _port = port;

    /// <summary>
    ///     Sends a message to the specified host and port.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void SendMessage(string message)
    {
        try
        {
            using var client = new TcpClient(_host, _port);
            var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            G.Logger.Error($"IPC Sender Error ({_host}:{_port}): {ex.Message}");
        }
    }
}