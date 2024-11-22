using Gantry.Services.MefLab.Abstractions;

namespace Gantry.Services.MefLab;

/// <summary>
///     Handles contracts on the client.
/// </summary>
public interface IMefLabClient : IMefLabSystem
{
    /// <summary>
    ///     Sends a contract to the server for processing.
    /// </summary>
    void SendContractToServer(string contractName, FileInfo contractFile);
}