using System.Text.RegularExpressions;

namespace Gantry.Services.EasyX.Extensions;

/// <summary>
///     Extension methods to aid broadcasting unique packets to players.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static partial class StringExtensions
{
    /// <summary>
    ///     Humanises a PascalCase phrase.
    /// </summary>
    public static string PascalCaseToSentence(this string input) =>
        string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : PascalCaseRegex().Replace(input, m => " " + m.Value);

    [GeneratedRegex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])")]
    private static partial Regex PascalCaseRegex();
}