using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem.v2.Abstractions;

internal interface IFileTypeWrapper
{
    IEnumerable<string> Extensions { get; }

    ModFileInfo Wrap(FileInfo file, FileScope scope);
}