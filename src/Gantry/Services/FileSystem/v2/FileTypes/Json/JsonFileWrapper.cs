using System.Collections.Generic;
using System.IO;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.FileTypes.Json
{
    [UsedImplicitly]
    internal sealed class JsonFileWrapper : IFileTypeWrapper
    {
        public IEnumerable<string> Extensions 
            => new List<string> { ".json", ".json5", ".data" };

        public ModFileInfo Wrap(FileInfo file, FileScope scope) 
            => new JsonFile(file, scope);
    }
}