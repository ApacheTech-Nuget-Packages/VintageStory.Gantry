using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Gantry.Services.EasyX.Extensions;

/// <summary>
///     Extension methods to aid broadcasting unique packets to players.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringExtensions
{
    /// <summary>
    ///     Humanises a PascalCase phrase.
    /// </summary>
    public static string PascalCaseToSentence(this string input) =>
        string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : Regex.Replace(input, "(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])", m => " " + m.Value);
}