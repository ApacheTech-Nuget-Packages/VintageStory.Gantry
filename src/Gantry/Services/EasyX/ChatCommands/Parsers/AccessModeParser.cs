using System.Diagnostics.Contracts;
using System.Globalization;
using ApacheTech.Common.Extensions.System;
using Gantry.Services.EasyX.ChatCommands.DataStructures;
using Vintagestory.API.Common;

namespace Gantry.Services.EasyX.ChatCommands.Parsers;

/// <summary>
///     Parses a string as a <see cref="AccessMode"/> value, allowing partial matches.
/// </summary>
/// <seealso cref="ArgumentParserBase" />
internal class AccessModeParser(string argName, bool isMandatoryArg) : ArgumentParserBase(argName, isMandatoryArg)
{
    /// <summary />
    public AccessMode? Mode { get; private set; }

    /// <inheritdoc />
    public override void PreProcess(TextCommandCallingArgs args)
    {
        Mode = null;
        base.PreProcess(args);
    }

    /// <inheritdoc />
    public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
    {
        var value = args.RawArgs.PopWord("");
        Mode = DirectParse(value) ?? FuzzyParse(value);
        return Mode is null
            ? EnumParseResult.Bad
            : EnumParseResult.Good;
    }

    /// <inheritdoc />
    public override object GetValue() => Mode;

    /// <inheritdoc />
    public override void SetValue(object data)
    {
        Mode = data switch
        {
            int index => (AccessMode)index,
            string value => DirectParse(value) ?? FuzzyParse(value),
            AccessMode mode => mode,
            _ => null
        };
    }

    [Pure]
    private static AccessMode? DirectParse(string value)
    {
        return Enum.TryParse(typeof(AccessMode), value, true, out var result)
            ? result.To<AccessMode>()
            : null;
    }

    [Pure]
    private static AccessMode? FuzzyParse(string value)
    {
        return value switch
        {
            _ when "disabled".StartsWith(value, true, CultureInfo.InvariantCulture) => AccessMode.Disabled,
            _ when "enabled".StartsWith(value, true, CultureInfo.InvariantCulture) => AccessMode.Enabled,
            _ when "whitelist".StartsWith(value, true, CultureInfo.InvariantCulture) => AccessMode.Whitelist,
            _ when "blacklist".StartsWith(value, true, CultureInfo.InvariantCulture) => AccessMode.Blacklist,
            _ when value.Equals("wl", StringComparison.InvariantCultureIgnoreCase) => AccessMode.Whitelist,
            _ => null,
        };
    }
}