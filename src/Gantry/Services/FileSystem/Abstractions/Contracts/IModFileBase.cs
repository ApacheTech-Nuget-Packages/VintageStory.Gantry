using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem.Abstractions.Contracts;

/// <summary>
///     Acts as a common ancestor for all mod file interfaces.
/// </summary>
public interface IModFileBase
{
    /// <summary>
    ///     Gets the type of the file, be it JSON, or Binary.
    /// </summary>
    /// <value>The type of the file.</value>
    public FileType FileType { get; }


    /// <summary>
    ///     Retrieves the underlying <see cref="FileInfo"/> object for the given file.
    /// </summary>
    /// <returns>A <see cref="FileInfo"/> object, instantiated with the given file's fully qualified path, as registered with the service.</returns>
    public FileInfo AsFileInfo();

    /// <summary>
    ///     Retrieves the absolute, fully qualified path to the file.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents the given file's fully qualified path, as registered with the service.</returns>
    public string Path { get; }
}