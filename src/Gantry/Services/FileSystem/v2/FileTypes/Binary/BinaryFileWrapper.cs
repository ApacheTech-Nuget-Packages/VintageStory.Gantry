using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;

namespace Gantry.Services.FileSystem.v2.FileTypes.Binary;

[UsedImplicitly]
internal sealed class BinaryFileWrapper : IFileTypeWrapper
{
    public IEnumerable<string> Extensions 
        => new List<string> { ".bin", ".dll", ".exe", ".dat" };

    public ModFileInfo Wrap(FileInfo file, FileScope scope) 
        => new BinaryFile(file, scope);
}