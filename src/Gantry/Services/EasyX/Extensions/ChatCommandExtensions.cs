using Gantry.Core.Helpers;
using Gantry.GameContent.ChatCommands.Parsers.Extensions;
using Gantry.Services.IO.Configuration.Abstractions;

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
    public static IChatCommand WithFeatureSpecifics(
        this IChatCommand subCommand, 
        Action<IChatCommand, CommandArgumentParsers> builder, 
        CommandArgumentParsers parsers)
    {
        builder(subCommand, parsers);
        return subCommand;
    }

    /// <summary>
    ///     Takes a feature settings class and builds sub-commands for each property decorated with the ChatCommandAttribute.
    /// </summary>
    /// <typeparam name="TServerSettings">The feature settings type.</typeparam>
    /// <param name="subCommand">The sub-command to add features to.</param>
    /// <param name="subCommandName">The name of the sub-command.</param>
    /// <param name="parsers">The collection of parsers that can be used to parse command arguments.</param>
    /// <param name="lang">The string translator.</param>
    /// <param name="handler">The handler to process the command.</param>
    public static IChatCommand WithFeatureSettings<TServerSettings>(
        this IChatCommand subCommand,
        string subCommandName,
        CommandArgumentParsers parsers,
        IStringTranslator lang,
        System.Func<TextCommandCallingArgs, string, TextCommandResult> handler)
        where TServerSettings : FeatureSettings<TServerSettings>, new()
    {
        foreach (var property in typeof(TServerSettings).GetProperties())
        {
            if (property.GetCustomAttribute<ChatCommandAttribute>() is not ChatCommandAttribute attribute) continue;
            var name = attribute.Name;
            var propertyName = property.Name;
            var propertyType = property.PropertyType;

            ICommandArgumentParser parser = propertyType switch
            {
                var t when t == typeof(bool) => parsers.Bool(name),
                var t when t == typeof(int) => parsers.Int(name),
                var t when t == typeof(int) && attribute.MinValue is int minInt && attribute.MaxValue is int maxInt => parsers.IntRange(name, minInt, maxInt),
                var t when t == typeof(float) => parsers.Float(name),
                var t when t == typeof(float) && attribute.MinValue is float minFloat && attribute.MaxValue is float maxFloat => parsers.FloatRange(name, minFloat, maxFloat),
                var t when t == typeof(double) => parsers.Double(name),
                var t when t == typeof(double) && attribute.MinValue is double minDouble && attribute.MaxValue is double maxDouble => parsers.DoubleRange(name, minDouble, maxDouble),
                var t when t == typeof(long) => parsers.Long(name),
                var t when t == typeof(string) => parsers.Word(name),
                var t when t == typeof(string) && attribute.WordList is string[] words => parsers.WordRange(name, words),
                _ => throw new NotSupportedException($"Property type '{propertyType.Name}' is not supported for command parsing.")
            };

            subCommand
                .BeginSubCommand(name)
                .WithArgs(parser)
                .WithDescription(lang.Translate($"{subCommandName}.{propertyName}", "Description"))
                .HandleWith(args => handler(args, propertyName))
                .EndSubCommand();
        }

        return subCommand;
    }
}