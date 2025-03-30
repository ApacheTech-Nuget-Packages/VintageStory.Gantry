using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.v2.Abstractions;

namespace Gantry.Services.FileSystem.v2.FileTypes.Json;

[UsedImplicitly]
internal sealed class JsonFileWrapper : IFileTypeWrapper
{
    public IEnumerable<string> Extensions => [".json", ".json5", ".data"];

    public ModFileInfo Wrap(FileInfo file, FileScope scope) => new JsonFile(file, scope);
}