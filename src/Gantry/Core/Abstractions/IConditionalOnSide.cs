namespace Gantry.Core.Abstractions;

/// <summary>
///     Represents a contract for attributes that conditionally apply to specific application sides (client, server, or universal).
/// </summary>
public interface IConditionalOnSide
{
    /// <summary>
    ///     The application side represented by this instance.
    /// </summary>
    EnumAppSide Side { get; }
}
