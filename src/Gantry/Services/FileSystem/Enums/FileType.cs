using System.ComponentModel;

namespace Gantry.Services.FileSystem.Enums;

/// <summary>
///     Specifies the type of a file saved to the user's game folder.
/// </summary>
public enum FileType
{
    /// <summary>
    ///     Denotes that a file is stored in clear-text JSON format.
    /// </summary>
    [Description("JSON File")] Json,

    /// <summary>
    ///     Denotes that a file is stored in binary ProtoBuf format.
    /// </summary>
    [Description("Binary (ProtofBuf) File")] Binary,

    /// <summary>
    ///     Denotes that a file is stored in clear text format.
    /// </summary>
    [Description("Text File")] Text
}