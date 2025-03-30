using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;

namespace Gantry.Services.FileSystem.v2.FileTypes.JsonSettings;

/// <summary>
///     Represents a JSON file, on the file system, used for configuring mod settings.
/// </summary>
/// <param name="file">A <see cref="FileInfo" /> representation of a file on the file system.</param>
/// <param name="scope">Determines where the file is stored on the file system.</param>
/// <seealso cref="ModFileInfo" />
/// <seealso cref="IJsonSettingsFile" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class JsonSettingsFile(FileInfo file, FileScope scope) : ModFileInfo(file, scope), IJsonSettingsFile;