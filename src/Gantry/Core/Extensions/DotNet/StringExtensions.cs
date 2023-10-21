using JetBrains.Annotations;

namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Extension methods to aid working with strings.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class StringExtensions
{
    /// <summary>
    ///     Guards against null strings by returning an empty string if the string is null.
    /// </summary>
    /// <param name="param">The string to check.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static string EmptyIfNull(this string param)
    {
        return param ?? string.Empty;
    }
}