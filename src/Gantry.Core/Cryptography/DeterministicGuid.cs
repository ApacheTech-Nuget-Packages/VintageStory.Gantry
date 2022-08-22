using System;
using System.Security.Cryptography;
using System.Text;
using Gantry.Core.Diagnostics;
using JetBrains.Annotations;

namespace Gantry.Core.Cryptography
{
    /// <summary>
    ///     Predictable, re-creatable <see cref="Guid"/>, produced from string input.  
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class DeterministicGuid
    {
        /// <summary>
        ///     Generates a deterministic <see cref="Guid"/> from a set of strings.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        public static Guid Create(params string[] data)
        {
            // use MD5 hash to get a 16-byte hash of the string.
            Guard.AgainstNullAndEmpty(nameof(data), data);
            using var provider = MD5.Create();
            var inputBytes = Encoding.Default.GetBytes(string.Concat(data));
            var hashBytes = provider.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }
}