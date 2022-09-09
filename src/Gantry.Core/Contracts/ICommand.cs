namespace Gantry.Core.Contracts
{
    /// <summary>
    ///     Represents a command, by way of The Command Pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     Executes this command.
        /// </summary>
        void Execute();
    }
}