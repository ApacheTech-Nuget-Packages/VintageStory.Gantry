namespace Gantry.Core.Extensions;

/// <summary>
///     Extension methods to aid working with chat commands.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ChatCommandExtensions
{
    /// <summary>
    ///     Returns all remaining arguments as single merged string, concatenated with spaces.
    /// </summary>
    /// <param name="args">The CmdArgs instance that called this extension method.</param>
    /// <param name="defaultValue">The default value to use, if nothing remains within the argument buffer.</param>
    public static string PopAll(this CmdArgs args, string defaultValue)
    {
        var retVal = args.PopAll();
        return string.IsNullOrWhiteSpace(retVal) ? defaultValue : retVal;
    }

    /// <summary>
    ///     Determines whether the specified chat command has a subcommand with the given name.
    /// </summary>
    /// <param name="command">The chat command to check for subcommands.</param>
    /// <param name="subCommandName">The name of the subcommand to look for.</param>
    /// <returns>
    ///     <see langword="true"/> if a subcommand with the specified name exists; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    ///     This method searches through the subcommands of the provided <paramref name="command"/> 
    ///     to determine if any match the specified <paramref name="subCommandName"/>.
    /// </remarks>
    public static bool HasSubCommand(this IChatCommand command, string subCommandName)
        => command.Subcommands.Any(p => p.Name == subCommandName);

    /// <summary>
    ///     Attempts to add a subcommand to the specified chat command if it doesn't already exist.
    /// </summary>
    /// <param name="command">The chat command to which the subcommand will be added.</param>
    /// <param name="subCommandName">The name of the subcommand to add.</param>
    /// <param name="subCommandBuilder">An action that builds the subcommand.</param>
    /// <param name="subCommand">The created or existing subcommand, returned as an output parameter.</param>
    /// <returns>
    ///     <c>true</c> if the subcommand was successfully added; otherwise, <c>false</c> if the subcommand already exists.
    /// </returns>
    /// <remarks>
    ///     This method checks if the subcommand with the specified <paramref name="subCommandName"/> 
    ///     already exists in the <paramref name="command"/>. If it does not exist, it begins a new subcommand, 
    ///     applies the <paramref name="subCommandBuilder"/> action to configure it, and returns <c>true</c>. 
    ///     If the subcommand already exists, the original <paramref name="command"/> is returned with <c>false</c>.
    ///     The <paramref name="subCommand"/> output parameter will hold the subcommand (whether newly created or existing).
    /// </remarks>
    public static bool TryAddSubCommand(this IChatCommand command, string subCommandName, Action<IChatCommand> subCommandBuilder, out IChatCommand subCommand)
    {
        subCommand = command;
        if (command.HasSubCommand(subCommandName)) return false;
        subCommand = command.BeginSubCommand(subCommandName);
        subCommandBuilder(subCommand);
        return true;
    }

    /// <summary>
    ///     Attempts to add a subcommand to the specified chat command if it doesn't already exist.
    /// </summary>
    /// <param name="command">The chat command to which the subcommand will be added.</param>
    /// <param name="subCommandName">The name of the subcommand to add.</param>
    /// <param name="subCommandBuilder">An action that builds the subcommand.</param>
    /// <returns>
    ///     <c>true</c> if the subcommand was successfully added; otherwise, <c>false</c> if the subcommand already exists.
    /// </returns>
    /// <remarks>
    ///     This method checks if the subcommand with the specified <paramref name="subCommandName"/> 
    ///     already exists in the <paramref name="command"/>. If it does not exist, it begins a new subcommand, 
    ///     applies the <paramref name="subCommandBuilder"/> action to configure it, and returns <c>true</c>. 
    ///     If the subcommand already exists, the method returns <c>false</c>.
    /// </remarks>
    public static bool TryAddSubCommand(this IChatCommand command, string subCommandName, Action<IChatCommand> subCommandBuilder)
        => command.TryAddSubCommand(subCommandName, subCommandBuilder, out _);
}