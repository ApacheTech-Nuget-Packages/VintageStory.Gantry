using Gantry.Services.IO.Abstractions;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.FileAdaptors;

/// <summary>
///     Represents a Text file, used by the mod. This class cannot be inherited.
/// </summary>
/// <seealso cref="ModFile" />
public sealed class TextModFile : ModFileBase, ITextModFile
{
    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonModFile"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public TextModFile(string filePath) : base(filePath)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonModFile"/> class.
    /// </summary>
    /// <param name="fileInfo">The file information.</param>
    public TextModFile(FileInfo fileInfo) : base(fileInfo)
    {
    }

    /// <summary>
    ///     Gets the type of the file.
    /// </summary>
    /// <value>The type of the file.</value>
    public override ModFileFormat FileFormat => ModFileFormat.Text;

    /// <summary>
    ///     Opens the file, reads all lines of text, and then closes the file.
    /// </summary>
    /// <returns>A <see cref="string" />, containing all lines of text within the file.</returns>
    public string ReadAllText()
        => File.ReadAllText(ModFileInfo.FullName);

    /// <summary>
    ///     Asynchronously opens the file, reads all lines of text, and then closes the file.
    /// </summary>
    /// <returns>A <see cref="string" />, containing all lines of text within the file.</returns>
    public Task<string> ReadAllTextAsync()
        => ModFileInfo.OpenText().ReadToEndAsync();
}