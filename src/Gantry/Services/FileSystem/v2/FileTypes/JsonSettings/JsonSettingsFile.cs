using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;

namespace Gantry.Services.FileSystem.v2.FileTypes.JsonSettings;

/// <summary>
///     Represents a JSON file, on the file system, used for configuring mod settings.
/// </summary>
/// <seealso cref="ModFileInfo" />
/// <seealso cref="IJsonSettingsFile" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class JsonSettingsFile : ModFileInfo, IJsonSettingsFile
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="JsonSettingsFile"/> class.
    /// </summary>
    /// <param name="file">A <see cref="FileInfo" /> representation of a file on the file system.</param>
    /// <param name="scope">Determines where the file is stored on the file system.</param>
    public JsonSettingsFile(FileInfo file, FileScope scope) : base(file, scope)
    {
    }
}