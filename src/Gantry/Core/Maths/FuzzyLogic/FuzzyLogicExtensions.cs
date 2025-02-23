namespace Gantry.Core.Maths.FuzzyLogic;

/// <summary>
///     Contains extension methods for fuzzy logic operations.
/// </summary>
public static class FuzzyLogicExtensions
{
    /// <summary>
    ///     Returns the Levenshtein distance between two strings.
    /// </summary>
    /// <param name="a">The source string.</param>
    /// <param name="b">The target string.</param>
    /// <returns>The Levenshtein distance between the two strings.</returns>
    public static int LevenshteinDistance(this string a, string b)
    {
        var costs = new int[a.Length + 1, b.Length + 1];

        for (var i = 0; i <= a.Length; i++)
            costs[i, 0] = i;

        for (var j = 0; j <= b.Length; j++)
            costs[0, j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                costs[i, j] = new[] {
                    costs[i - 1, j] + 1,
                    costs[i, j - 1] + 1,
                    costs[i - 1, j - 1] + cost
                }.Min();
            }
        }
        return costs[a.Length, b.Length];
    }
}
