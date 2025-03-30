using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem.v2.DataStructures;

/// <summary>
///     Describes a file with its scope, and <see cref="FileInfo"/> representation.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class FileDescriptor
{
    /// <summary>
    ///     The name of the file, including the file extension.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    ///     Determines where the file is stored on the file system.
    /// </summary>
    public FileScope Scope { get; }

    /// <summary>
    ///     The absolute path of the file, on the file system.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     A <see cref="FileInfo"/> representation of a file on the file system.
    /// </summary>
    public FileInfo File { get; }

    internal FileDescriptor(string fileName, FileScope scope, bool gantryFile = false)
    {
        FileName = fileName;
        Scope = scope;
        Path = ModPaths.GetScopedPath(fileName, scope, gantryFile);
        File = new FileInfo(Path);
    }
}