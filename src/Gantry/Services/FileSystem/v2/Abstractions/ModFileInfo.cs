using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.Abstractions;

/// <summary>
///     A wrapper of a <see cref="FileInfo" /> for a specific file on on the filesystem. This class cannot be inherited.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ModFileInfo
{
    /// <summary>
    ///     Determines where the file is stored on the file system.
    /// </summary>
    public FileScope Scope { get; private set; }

    /// <summary>
    ///     A <see cref="FileInfo"/> representation of a file on the file system.
    /// </summary>
    public FileInfo File { get; private set; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="ModFileInfo"/> class.
    /// </summary>
    /// <param name="file">A <see cref="FileInfo"/> representation of a file on the file system.</param>
    /// <param name="scope">Determines where the file is stored on the file system.</param>
    protected internal ModFileInfo(FileInfo file, FileScope scope) 
        => (File, Scope) = (file, scope);

    internal static T Create<T>(FileInfo file, FileScope scope) where T : ModFileInfo, new() 
        => new() { File = file, Scope = scope };

    internal static T FromDescriptor<T>(FileDescriptor descriptor) where T : ModFileInfo, new() 
        => new() { File = descriptor.File, Scope = descriptor.Scope };

    internal static ModFileInfo FromDescriptor(FileDescriptor descriptor)
        => new(descriptor.File, descriptor.Scope);
}