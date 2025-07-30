using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Abstractions.Contracts;

/// <summary>
///     Provides a means for handling files, including embedded resources, used within a mod.
/// </summary>
public interface IFileSystemService : IDisposable
{
    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return an <see cref="object"/> representation of the file, on disk.</returns>
    object GetRegisteredFile(string fileName);

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return an <see cref="IJsonModFile"/> representation of the file, on disk.</returns>
    IJsonModFile GetJsonFile(string fileName);

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return an <see cref="IBinaryModFile"/> representation of the file, on disk.</returns>
    IBinaryModFile GetBinaryFile(string fileName);

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    TModel ParseEmbeddedJsonFile<TModel>(string fileName);

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    IEnumerable<TModel> ParseEmbeddedJsonArrayFile<TModel>(string fileName);

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <param name="pathToFile">The full path of the file, including file extension.</param>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    TModel ParseJsonFile<TModel>(string pathToFile);

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return an <see cref="ITextModFile"/> representation of the file, on disk.</returns>
    ITextModFile GetTextFile(string fileName);

    /// <summary>
    ///     Retrieves a file that has previously been registered with the FileSystem Service.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Return an <typeparamref name="TFileType"/> representation of the file, on disk.</returns>
    TFileType GetRegisteredFile<TFileType>(string fileName) where TFileType : IModFileBase;

    /// <summary>
    ///     Registers a file with the FileSystem Service. This will copy a default implementation of the file from:
    ///     <br/>• An embedded resource.
    ///     <br/>• The mod's unpack cache directory.
    ///     <br/>• The mod's assets folder.
    ///     <br/>If no default implementation can be found, a new file is created, at the correct location.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <param name="scope">The scope of the file, be it global, per-world, or gantry.</param>
    public IFileSystemService RegisterDataFile(string fileName, ModFileScope scope)
        => RegisterFile(fileName, ModFileType.Data, scope);

    /// <summary>
    ///     Registers a file with the FileSystem Service. This will copy a default implementation of the file from:
    ///     <br/>• An embedded resource.
    ///     <br/>• The mod's unpack cache directory.
    ///     <br/>• The mod's assets folder.
    ///     <br/>If no default implementation can be found, a new file is created, at the correct location.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <param name="type">The type of the file, such as Assets, Settings, or Data.</param>
    /// <param name="scope">The scope of the file, be it global, per-world, or gantry.</param>
    public IFileSystemService RegisterFile(string fileName, ModFileType type, ModFileScope scope);

    /// <summary>
    ///     Registers a settings file with the FileSystem Service. This will copy a default implementation of the file from:
    ///      • An embedded resource.
    ///      • The mod's unpack directory.
    ///      • The mod's assets folder.
    /// 
    ///     If no default implementation can be found, a new file is created, at the correct location.
    /// </summary>
    /// <param name="fileName">The name of the file, including file extension.</param>
    /// <param name="scope">The scope of the file, be it global, or per-world.</param>
    void RegisterSettingsFile(string fileName, ModFileScope scope);

    /// <summary>
    ///     Registers the default settings files for the mod.
    /// </summary>
    internal void RegisterDefaultSettingsFiles();
}