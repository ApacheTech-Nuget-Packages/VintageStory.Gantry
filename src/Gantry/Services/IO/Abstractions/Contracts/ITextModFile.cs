namespace Gantry.Services.IO.Abstractions.Contracts;

/// <summary>
///     Represents a Text file on the filesystem.
/// </summary>
public interface ITextModFile : IModFileBase
{
    /// <summary>
    ///     Opens the file, reads all lines of text, and then closes the file.
    /// </summary>
    /// <returns>A <see cref="string" />, containing all lines of text within the file.</returns>
    public string ReadAllText();

    /// <summary>
    ///     Asynchronously opens the file, reads all lines of text, and then closes the file.
    /// </summary>
    /// <returns>A <see cref="string" />, containing all lines of text within the file.</returns>
    public Task<string> ReadAllTextAsync();
}