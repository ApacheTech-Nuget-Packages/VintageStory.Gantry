namespace Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions
{
    /// <summary>
    ///     Represents a packet that contains a string message from one app-side to the other.
    /// </summary>
    public interface IMessageProvider
    {
        /// <summary>
        ///     The message to display to the user.
        /// </summary>
        string Message { get; }
    }
}