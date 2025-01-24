namespace Gantry.Services.EasyX.Extensions;

/// <summary>
///     Extension methods to aid building sub-commands for EasyX features.
/// </summary>
public static class ChatCommandExtensions
{
    /// <summary>
    ///     Adds feature specific sub-commands to the feature command. 
    /// </summary>
    /// <param name="subCommand">The sub-command to add features to.</param>
    /// <param name="builder">The extra details to add to the command.</param>
    /// <param name="parsers">The collection of parsers that can be used to parse command arguments.</param>
    /// <returns></returns>
    public static IChatCommand WithFeatureSpecifics(
        this IChatCommand subCommand, 
        Action<IChatCommand, CommandArgumentParsers> builder, 
        CommandArgumentParsers parsers)
    {
        builder(subCommand, parsers);
        return subCommand;
    }
}