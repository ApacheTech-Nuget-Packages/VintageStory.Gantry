using Gantry.Services.MefLab.Abstractions;

namespace Gantry.Services.MefLab;

/// <summary>
///     Handles contracts on the server.
/// </summary>
public interface IMefLabServer : IMefLabSystem
{
    /// <summary>
    ///     Sends a contract to the client for processing.
    /// </summary>
    void SendContractToClient(string contractName, FileInfo contractFile);
}
