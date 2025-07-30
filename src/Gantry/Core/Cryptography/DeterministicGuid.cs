using System.Security.Cryptography;
using System.Text;

namespace Gantry.Core.Cryptography;

/// <summary>
///     Predictable, re-creatable <see cref="Guid"/>, produced from string input.  
/// </summary>
public static class DeterministicGuid
{
    /// <summary>
    ///     Generates a deterministic <see cref="Guid"/> from a set of strings.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    public static Guid Create(params string[] data)
    {
        // use MD5 hash to get a 16-byte hash of the string.
        if (data is null || data.Length == 0)
        {
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));
        }
        var inputBytes = Encoding.Default.GetBytes(string.Concat(data));
        var hashBytes = MD5.HashData(inputBytes);
        return new Guid(hashBytes);
    }
}