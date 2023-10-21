using JetBrains.Annotations;
using Vintagestory.API.Common;

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
}