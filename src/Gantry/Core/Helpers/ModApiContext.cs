#pragma warning disable CS8603 // Possible null reference return.

using Gantry.Core.Abstractions;

namespace Gantry.Core.Helpers;

/// <inheritdoc />
public class ModApiContext : IModApiContext
{
    private readonly AsyncLocal<ClientMain?> _clientMain = new();
    private readonly AsyncLocal<ServerMain?> _serverMain = new();
    private readonly Thread? _serverThread;
    private readonly Thread? _clientThread;

    internal ModApiContext(ICoreGantryAPI core)
    {
        switch (core.Uapi.Side)
        {
            case EnumAppSide.Client:
                _clientMain.Value = core.Uapi.World as ClientMain;
                _clientThread = Thread.CurrentThread;
                core.Logger.VerboseDebug("ModApiContext: Added ClientMain (Thread ID: {0}).", _clientThread.ManagedThreadId);
                break;
            case EnumAppSide.Server:
                _serverMain.Value = core.Uapi.World as ServerMain;
                _serverThread = Thread.CurrentThread;
                core.Logger.VerboseDebug("ModApiContext: Added ServerMain (Thread ID: {0}).", _serverThread.ManagedThreadId);
                break;
            case EnumAppSide.Universal:
            default:
                throw new UnreachableException();
        }
    }

    /// <inheritdoc />
    public Thread MainThread => Side.IsClient() ? _clientThread : _serverThread;

    /// <inheritdoc />
    public ICoreClientAPI Client => ClientMain?.Api as ICoreClientAPI;

    /// <inheritdoc />
    public ICoreServerAPI Server => ServerMain?.Api as ICoreServerAPI;

    /// <inheritdoc />
    public ICoreAPI Current => Side.IsClient() ? Client : Server;

    /// <inheritdoc />
    public ClientMain ClientMain => _clientMain.Value;

    /// <inheritdoc />
    public ServerMain ServerMain => _serverMain.Value;

    /// <inheritdoc />
    public EnumAppSide Side
    {
        get
        {
            try
            {
                return ServerMain is not null
                    ? EnumAppSide.Server
                    : EnumAppSide.Client;
            }
            catch
            {
                return EnumAppSide.Universal;
            }
        }
    }
}