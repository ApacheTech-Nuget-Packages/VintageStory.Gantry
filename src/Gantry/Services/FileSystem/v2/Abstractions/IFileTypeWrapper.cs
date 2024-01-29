using Gantry.Services.FileSystem.v2.DataStructures;

namespace Gantry.Services.FileSystem.v2.Abstractions;

internal interface IFileTypeWrapper
{
    IEnumerable<string> Extensions { get; }

    ModFileInfo Wrap(FileInfo file, FileScope scope);
}