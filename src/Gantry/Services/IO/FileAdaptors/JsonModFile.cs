using Gantry.Services.IO.Abstractions;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.DataStructures;
using Gantry.Services.IO.Extensions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Datastructures;

namespace Gantry.Services.IO.FileAdaptors;

/// <summary>
///     Represents a JSON file, used by the mod. This class cannot be inherited.
/// </summary>
/// <seealso cref="ModFile" />
public sealed class JsonModFile : ModFile, IJsonModFile
{
    private readonly ILogger _logger;

    /// <summary>
    ///     Gets the json serialiser settings.
    /// </summary>
    public static JsonSerializerSettings JsonSerialiserSettings { get; } = new()
    {
        Converters = [new StringEnumConverter()],
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonModFile"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public JsonModFile(string filePath, ILogger logger) : base(filePath)
    {
        _logger = logger;
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonModFile"/> class.
    /// </summary>
    /// <param name="fileInfo">The file information.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public JsonModFile(FileInfo fileInfo, ILogger logger) : base(fileInfo)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Gets the type of the file.
    /// </summary>
    /// <value>The type of the file.</value>
    public override ModFileFormat FileFormat => ModFileFormat.Json;

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public override TModel ParseAs<TModel>()
    {
        var json = File.ReadAllText(ModFileInfo.FullName);
        var resource = JsonConvert.DeserializeObject<TModel>(json, JsonSerialiserSettings)
            ?? throw new JsonSerializationException();
        return resource;
    }

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public override Task<TModel> ParseAsAsync<TModel>() 
        => Task.Factory.StartNew(ParseAs<TModel>);

    /// <summary>
    ///     Deserialises the specified file as a collection of a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <see cref="IEnumerable{TModel}" />, populated with data from this file.</returns>
    public override IEnumerable<TModel> ParseAsMany<TModel>()
    {
        try
        {
            var fileContent = File.ReadAllText(ModFileInfo.FullName);
            var token = JToken.Parse(fileContent);
            if (token.Type == JTokenType.Object) return [];
            if (token.Type == JTokenType.Array) return token.ToObject<IEnumerable<TModel>>() ?? [];
            return [];
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to parse JSON file: {ModFileInfo.FullName}");
            _logger.Error(ex);
            return [];
        }
    }

    /// <summary>
    ///     Deserialises the specified file as a collection of a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <see cref="IEnumerable{TModel}" />, populated with data from this file.</returns>
    public override Task<IEnumerable<TModel>> ParseAsManyAsync<TModel>() 
        => Task.Factory.StartNew(ParseAsMany<TModel>);

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override void SaveFrom<TModel>(TModel instance) 
        => SaveFrom(JsonConvert.SerializeObject(instance, JsonSerialiserSettings));

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override void SaveFromList<TModel>(IEnumerable<TModel> instance) 
        => SaveFrom(JsonConvert.SerializeObject(instance, JsonSerialiserSettings));

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override Task SaveFromListAsync<TModel>(IEnumerable<TModel> instance) 
        => Task.Factory.StartNew(() => SaveFromList(instance));

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="file">The file to save.</param>
    public void SaveFrom(IJsonModFile file) 
        => SaveFrom(file.ReadAllText());

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    /// <returns>Task.</returns>
    public override Task SaveFromAsync<TModel>(TModel instance) 
        => Task.Factory.StartNew(() => SaveFrom(instance));

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    /// <returns>Task.</returns>
    public override Task SaveFromAsync<TModel>(IEnumerable<TModel> collection) 
        => Task.Factory.StartNew(() => SaveFrom(collection));

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
        => ModFileInfo.ReadAllTextAsync();

    /// <summary>
    ///     Parses the file into Vintage Story's bespoke JsonObject wrapper.
    /// </summary>
    /// <returns>An instance of type <see cref="JsonObject" />, populated with data from this file.</returns>
    public Task<JsonObject> ParseAsJsonObjectAsync() 
        => Task.Factory.StartNew(ParseAsJsonObject);

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    public override void SaveFrom<TModel>(IEnumerable<TModel> collection) 
        => SaveFrom(JsonConvert.SerializeObject(collection, JsonSerialiserSettings));

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="json">The serialised JSON string to save to a single file.</param>
    public void SaveFrom(string json)
    {
        try
        {
            File.WriteAllText(ModFileInfo.FullName, json);
        }
        catch (IOException e)
        {
            _logger.Warning($"Failed to save JSON file: {ModFileInfo.FullName}");
            _logger.Warning(e.Message);
        }
    }

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="json">The serialised JSON string to save to a single file.</param>
    /// <returns>Task.</returns>
    public Task SaveFromAsync(string json) 
        => ModFileInfo.WriteAllTextAsync(json);

    /// <summary>
    ///     Parses the file into Vintage Story's bespoke JsonObject wrapper.
    /// </summary>
    /// <returns>An instance of type <see cref="JsonObject" />, populated with data from this file.</returns>
    public JsonObject ParseAsJsonObject() 
        => JsonObject.FromJson(File.ReadAllText(ModFileInfo.FullName));
}