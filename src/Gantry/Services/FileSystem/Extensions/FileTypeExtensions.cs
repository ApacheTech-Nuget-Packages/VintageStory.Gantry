using Gantry.Services.FileSystem.Abstractions;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.FileAdaptors;

namespace Gantry.Services.FileSystem.Extensions;

/// <summary>
///     Extension methods to aid the use of <see cref="FileType"/> enums.
/// </summary>
public static class FileTypeExtensions
{
    private static readonly Dictionary<string, FileType> Types = new()
    {
        { ".json", FileType.Json },
        { ".data", FileType.Json },
        { ".dat", FileType.Binary },
        { ".bin", FileType.Binary },
        { ".dll", FileType.Binary },
        { ".txt", FileType.Text },
        { ".md", FileType.Text },
    };

    /// <summary>
    ///     Parses the type of the file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns></returns>
    public static FileType ParseFileType(this FileSystemInfo file)
    {
        return Types.TryGetValue(file.Extension, out var extension) 
            ? extension
            : FileType.Binary;
    }

    /// <summary>
    ///     Creates the mod file wrapper.
    /// </summary>
    /// <param name="file">The file to wrap.</param>
    /// <returns></returns>
    public static ModFileBase CreateModFileWrapper(this FileInfo file)
    {
        var fileType = ParseFileType(file);
        return fileType switch
        {
            FileType.Json => new JsonModFile(file),
            FileType.Binary => new BinaryModFile(file),
            FileType.Text => new TextModFile(file),
            _ => throw new ArgumentOutOfRangeException(nameof(fileType))
        };
    }
}