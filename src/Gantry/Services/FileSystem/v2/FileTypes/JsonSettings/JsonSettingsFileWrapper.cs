using System.Collections.Generic;
using System.IO;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.FileTypes.JsonSettings
{
    [UsedImplicitly]
    internal sealed class JsonSettingsFileWrapper : IFileTypeWrapper
    {
        public IEnumerable<string> Extensions
            => new List<string> { ".settings.json" };

        public ModFileInfo Wrap(FileInfo file, FileScope scope)
            => new JsonSettingsFile(file, scope);
    }
}