using Vintagestory.API.Common;

namespace Gantry.Core.Brighter.Abstractions;

/// <summary>
///     Represents a command that is executed on behalf of a specific player.
/// </summary>
public interface IPlayerSpecificCommand
{
    /// <summary>
    ///     The player that this command is being executed on behalf of.
    /// </summary>
    IPlayer Player { get; set; }
}