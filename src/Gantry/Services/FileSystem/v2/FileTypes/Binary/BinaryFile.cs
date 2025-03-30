using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.v2.Abstractions;

namespace Gantry.Services.FileSystem.v2.FileTypes.Binary;

/// <summary>
///     Represents a binary file, on the file system.
/// </summary>
/// <param name="file">A <see cref="FileInfo" /> representation of a file on the file system.</param>
/// <param name="scope">Determines where the file is stored on the file system.</param>
/// <seealso cref="ModFileInfo" />
/// <seealso cref="IBinaryFile" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BinaryFile(FileInfo file, FileScope scope) : ModFileInfo(file, scope), IBinaryFile;