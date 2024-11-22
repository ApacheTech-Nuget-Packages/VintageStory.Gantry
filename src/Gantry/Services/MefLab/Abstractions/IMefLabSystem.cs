namespace Gantry.Services.MefLab.Abstractions;

/// <summary>
///     Mediates MEF contracts between client and server.
/// </summary>
public interface IMefLabSystem
{
    /// <summary>
    ///     The name of the MefLab network channel.
    /// </summary>
    const string ChannelName = "MefLab";
}