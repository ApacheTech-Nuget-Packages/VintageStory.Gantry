using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;

namespace Gantry.Services.FileSystem.v2.FileTypes.Json;

/// <summary>
///     Represents a JSON file, on the file system.
/// </summary>
/// <seealso cref="ModFileInfo" />
/// <seealso cref="IJsonFile" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class JsonFile : ModFileInfo, IJsonFile
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="JsonFile"/> class.
    /// </summary>
    /// <param name="file">A <see cref="FileInfo" /> representation of a file on the file system.</param>
    /// <param name="scope">Determines where the file is stored on the file system.</param>
    public JsonFile(FileInfo file, FileScope scope) : base(file, scope)
    {
    }
}