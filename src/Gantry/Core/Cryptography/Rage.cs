using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace Gantry.Core.Cryptography;

/// <summary>
///     Represents a class for generating hash keys compatible with GTA 5 file formats.
/// </summary>
/// <remarks>
///     Copied directly from the game -- performs uppercase to lowercase conversions amongst other things.
/// </remarks>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class Rage
{
    /// <summary>
    ///     Gets the hash key of the specified sub string, using an initial hash key value.
    /// </summary>
    /// <param name="str">The string used to generate a hash key.</param>
    /// <param name="initialHash">The initial hash key value used to generate the result.</param>
    /// <returns>A partial hash key compatible with GTA 5.</returns>
    private static uint GetHashKeySubString(string str, uint initialHash)
    {
        var hash = initialHash;
        foreach (var t in str)
        {
            hash += t;
            hash += hash << 10;
            hash ^= hash >> 6;
        }
        return hash;
    }

    /// <summary>
    ///     Finalises the hash key of the specified string, using an initial hash key value.
    /// </summary>
    /// <param name="str">The string used to generate a hash key.</param>
    /// <param name="initialHash">The initial hash key value used to generate the result.</param>
    /// <returns>A hash key compatible with GTA 5.</returns>
    private static uint GetHashKeyFinalize(string str, uint initialHash)
    {
        var hash = GetHashKeySubString(str, initialHash);
        hash += hash << 3;
        hash ^= hash >> 11;
        hash += hash << 15;
        return hash;
    }

    /// <summary>
    ///     Gets the hash key of the specified string, using an initial hash key value.
    /// </summary>
    /// <param name="str">The string used to generate a hash key.</param>
    /// <param name="initialHash">The initial hash key value used to generate the result.</param>
    /// <returns>A hash key compatible with GTA 5.</returns>
    public static string JOAAT(string str, uint initialHash = 0) 
        => GetHashKeyFinalize(str, initialHash).ToString("X2");

    /// <summary>
    ///     Gets the hash key of the specified string concatenated with an additional string, using an initial hash key value.
    /// </summary>
    /// <param name="str">The string used to generate a hash key.</param>
    /// <param name="concat">The additional string to concatenate to the input string.</param>
    /// <param name="initialHash">The initial hash key value used to generate the result.</param>
    /// <returns>A hash key compatible with GTA 5.</returns>
    public static string JOAAT(string str, string concat, uint initialHash = 0) 
        => GetHashKeyFinalize(concat, GetHashKeySubString(str, initialHash)).ToString("X2");
}