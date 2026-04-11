namespace Gantry.Core.Helpers;

/// <summary>
///     A monad that holds a value of a specific type, for both the client and server sides.
/// </summary>
public interface ISidedInstance
{
    /// <summary>
    ///     Determines if a value has been set for either the client or server side.
    /// </summary>
    bool HasValue(EnumAppSide side);

    /// <summary>
    ///     Sets the value for the current side (client or server).
    /// </summary>
    /// <param name="side">The side for which to set the value.</param>
    /// <param name="value">The value to set.</param>
    void Set(EnumAppSide side, object? value);
}
