using System.Diagnostics.Contracts;
using System.Globalization;
using Gantry.Services.FileSystem.Enums;

namespace Gantry.Core.GameContent.ChatCommands.Parsers;

/// <summary>
///     Parses a string as a <see cref="FileScope"/> value, allowing partial matches.
/// </summary>
/// <seealso cref="ArgumentParserBase" />
public class FileScopeParser(string argName, bool isMandatoryArg) : ArgumentParserBase(argName, isMandatoryArg)
{
    /// <summary>
    ///     Specifies whether to use world, or global settings to save feature settings to.
    /// </summary>
    public FileScope? Scope { get; private set; }

    /// <inheritdoc />
    public override void PreProcess(TextCommandCallingArgs args)
    {
        Scope = null;
        base.PreProcess(args);
    }

    /// <inheritdoc />
    public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults>? onReady = null)
    {
        var value = args.RawArgs.PopWord("");
        Scope = DirectParse(value) ?? FuzzyParse(value);
        return Scope is null
            ? EnumParseResult.Bad
            : EnumParseResult.Good;
    }

    /// <inheritdoc />
    public override object? GetValue() => Scope;

    /// <inheritdoc />
    public override void SetValue(object data)
    {
        Scope = data switch
        {
            int index => (FileScope)index,
            string value => DirectParse(value) ?? FuzzyParse(value),
            FileScope scope => scope,
            _ => null
        };
    }

    [Pure]
    private static FileScope? DirectParse(string value)
    {
        return Enum.TryParse(typeof(FileScope), value, true, out var result)
            ? result.To<FileScope>()
            : null;
    }

    [Pure]
    private static FileScope? FuzzyParse(string value)
    {
        return value switch
        {
            _ when "world".StartsWith(value, true, CultureInfo.InvariantCulture) => FileScope.World,
            _ when "global".StartsWith(value, true, CultureInfo.InvariantCulture) => FileScope.Global,
            _ => null,
        };
    }
}