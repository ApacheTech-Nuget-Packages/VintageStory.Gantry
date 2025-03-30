using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.v2.Abstractions;

namespace Gantry.Services.FileSystem.v2.FileTypes.Binary;

[UsedImplicitly]
internal sealed class BinaryFileWrapper : IFileTypeWrapper
{
    public IEnumerable<string> Extensions => [".bin", ".dll", ".exe", ".dat"];

    public ModFileInfo Wrap(FileInfo file, FileScope scope) => new BinaryFile(file, scope);
}