using Vintagestory.API.Datastructures;

namespace Gantry.Services.IO.Abstractions.Contracts;

/// <summary>
///     Represents a JSON file on the filesystem.
/// </summary>
public interface IJsonModFile : IModFile, ITextModFile
{
    /// <summary>
    ///     Parses the file into Vintage Story's bespoke JsonObject wrapper.
    /// </summary>
    /// <returns>An instance of type <see cref="JsonObject"/>, populated with data from this file.</returns>
    public JsonObject ParseAsJsonObject();

    /// <summary>
    ///     Parses the file into Vintage Story's bespoke JsonObject wrapper.
    /// </summary>
    /// <returns>An instance of type <see cref="JsonObject"/>, populated with data from this file.</returns>
    public Task<JsonObject> ParseAsJsonObjectAsync();

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="file">The file to save.</param>
    public void SaveFrom(IJsonModFile file);

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="json">The serialised JSON string to save to a single file.</param>
    public void SaveFrom(string json);

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="json">The serialised JSON string to save to a single file.</param>
    public Task SaveFromAsync(string json);
}