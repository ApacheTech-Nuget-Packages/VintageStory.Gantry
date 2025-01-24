using Gantry.Services.FileSystem.Abstractions;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.Extensions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Datastructures;

namespace Gantry.Services.FileSystem.FileAdaptors;

/// <summary>
///     Represents a JSON file, used by the mod. This class cannot be inherited.
/// </summary>
/// <seealso cref="ModFile" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class JsonModFile : ModFile, IJsonModFile
{
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
    public JsonModFile(string filePath) : base(filePath)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonModFile"/> class.
    /// </summary>
    /// <param name="fileInfo">The file information.</param>
    public JsonModFile(FileInfo fileInfo) : base(fileInfo)
    {
    }

    /// <summary>
    ///     Gets the type of the file.
    /// </summary>
    /// <value>The type of the file.</value>
    public override FileType FileType => FileType.Json;

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public override TModel ParseAs<TModel>()
    {
        try
        {
            var json = File.ReadAllText(ModFileInfo.FullName);
            return JsonConvert.DeserializeObject<TModel>(json, JsonSerialiserSettings);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public override Task<TModel> ParseAsAsync<TModel>()
    {
        return Task.Factory.StartNew(ParseAs<TModel>);
    }

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
            // Read file content
            var fileContent = File.ReadAllText(ModFileInfo.FullName);

            // Parse content as JToken
            var token = JToken.Parse(fileContent);

            // Check if the token is an object (i.e., "{}") and treat it as an empty array
            if (token.Type == JTokenType.Object)
            {
                return [];
            }

            // If the token is an array, deserialise it into IEnumerable<TModel>
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<IEnumerable<TModel>>() ?? [];
            }

            // If the token is neither an object nor an array, return an empty collection
            return [];
        }
        catch (Exception)
        {
            // Return an empty collection on error
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
    {
        return Task.Factory.StartNew(ParseAsMany<TModel>);
    }

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override void SaveFrom<TModel>(TModel instance)
    {
        var json = JsonConvert.SerializeObject(instance, JsonSerialiserSettings);
        SaveFrom(json);
    }

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override void SaveFromList<TModel>(IEnumerable<TModel> instance)
    {
        var json = JsonConvert.SerializeObject(instance, JsonSerialiserSettings);
        SaveFrom(json);
    }

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override Task SaveFromListAsync<TModel>(IEnumerable<TModel> instance)
    {
        return Task.Factory.StartNew(() => SaveFromList(instance));
    }

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="file">The file to save.</param>
    public void SaveFrom(IJsonModFile file)
    {
        SaveFrom(file.ReadAllText());
    }

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    /// <returns>Task.</returns>
    public override Task SaveFromAsync<TModel>(TModel instance)
    {
        return Task.Factory.StartNew(() => SaveFrom(instance));
    }

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    /// <returns>Task.</returns>
    public override Task SaveFromAsync<TModel>(IEnumerable<TModel> collection)
    {
        return Task.Factory.StartNew(() => SaveFrom(collection));
    }

    /// <summary>
    ///     Opens the file, reads all lines of text, and then closes the file.
    /// </summary>
    /// <returns>A <see cref="string" />, containing all lines of text within the file.</returns>
    public string ReadAllText()
    {
        return File.ReadAllText(ModFileInfo.FullName);
    }

    /// <summary>
    ///     Asynchronously opens the file, reads all lines of text, and then closes the file.
    /// </summary>
    /// <returns>A <see cref="string" />, containing all lines of text within the file.</returns>
    public Task<string> ReadAllTextAsync()
    {
        return ModFileInfo.ReadAllTextAsync();
    }

    /// <summary>
    ///     Parses the file into Vintage Story's bespoke JsonObject wrapper.
    /// </summary>
    /// <returns>An instance of type <see cref="JsonObject" />, populated with data from this file.</returns>
    public Task<JsonObject> ParseAsJsonObjectAsync()
    {
        return Task.Factory.StartNew(ParseAsJsonObject);
    }

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    public override void SaveFrom<TModel>(IEnumerable<TModel> collection)
    {
        var json = JsonConvert.SerializeObject(collection, JsonSerialiserSettings);
        SaveFrom(json);
    }

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
            ApiEx.Logger.Warning($"[Gantry] Failed to save JSON file: {ModFileInfo.FullName}");
            ApiEx.Logger.Warning(e.Message);
        }
    }

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <param name="json">The serialised JSON string to save to a single file.</param>
    /// <returns>Task.</returns>
    public Task SaveFromAsync(string json)
    {
        return ModFileInfo.WriteAllTextAsync(json);
    }

    /// <summary>
    ///     Parses the file into Vintage Story's bespoke JsonObject wrapper.
    /// </summary>
    /// <returns>An instance of type <see cref="JsonObject" />, populated with data from this file.</returns>
    public JsonObject ParseAsJsonObject()
    { 
        return JsonObject.FromJson(File.ReadAllText(ModFileInfo.FullName));
    }
}