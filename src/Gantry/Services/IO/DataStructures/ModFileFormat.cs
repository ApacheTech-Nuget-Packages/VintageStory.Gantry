using System.ComponentModel;

namespace Gantry.Services.IO.DataStructures;

/// <summary>
///     Specifies the type of a file saved to the user's game folder.
/// </summary>
public enum ModFileFormat
{
    /// <summary>
    ///     Denotes that a file is stored in clear-text JSON format.
    /// </summary>
    [Description("JSON File")] 
    Json,

    /// <summary>
    ///     Denotes that a file is stored in binary ProtoBuf format.
    /// </summary>
    [Description("Binary (ProtofBuf) File")] 
    Binary,

    /// <summary>
    ///     Denotes that a file is stored in clear text format.
    /// </summary>
    /// 
    [Description("Text File")] 
    Text
}