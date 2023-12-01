using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.FileTypes.Binary;

/// <summary>
///     Extension methods to aid providing binary files to the user.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BinaryFileProviderExtensions
{
    /// <summary>
    ///     Gets a binary file, previously registered with the file provider.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="scope">The scope of the file.</param>
    /// <returns>A <see cref="IBinaryFile"/> representation of the file, on the file system.</returns>
    public static IBinaryFile GetBinaryFile(this IFileProvider provider, string fileName, FileScope scope) 
        => provider.Wrap<IBinaryFile>(fileName, scope);
}