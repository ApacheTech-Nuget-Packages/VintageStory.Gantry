using Gantry.Core.Abstractions;
using Gantry.Services.IO.Abstractions;
using Gantry.Services.IO.DataStructures;
using Gantry.Services.IO.FileAdaptors;

namespace Gantry.Services.IO.Extensions;

/// <summary>
///     Extension methods to aid the use of <see cref="ModFileFormat"/> enums.
/// </summary>
public static class ModFileFormatExtensions
{
    private static readonly Dictionary<string, ModFileFormat> _types = new()
    {
        { ".json", ModFileFormat.Json },
        { ".data", ModFileFormat.Json },
        { ".dat", ModFileFormat.Binary },
        { ".bin", ModFileFormat.Binary },
        { ".dll", ModFileFormat.Binary },
        { ".txt", ModFileFormat.Text },
        { ".md", ModFileFormat.Text },
    };

    /// <summary>
    ///     Parses the type of the file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns></returns>
    public static ModFileFormat ParseModFileFormat(this FileSystemInfo file)
    {
        return _types.TryGetValue(file.Extension, out var extension) 
            ? extension
            : ModFileFormat.Binary;
    }

    /// <summary>
    ///     Creates the mod file wrapper.
    /// </summary>
    /// <param name="file">The file to wrap.</param>
    /// <param name="core">The core gantry API instance.</param>
    /// <returns></returns>
    public static ModFileBase CreateModFileWrapper(this FileInfo file, ICoreGantryAPI core)
    {
        var fileType = file.ParseModFileFormat();
        return fileType switch
        {
            ModFileFormat.Json => new JsonModFile(file, core.Logger),
            ModFileFormat.Binary => new BinaryModFile(file),
            ModFileFormat.Text => new TextModFile(file),
            _ => throw new ArgumentOutOfRangeException(nameof(file))
        };
    }
}