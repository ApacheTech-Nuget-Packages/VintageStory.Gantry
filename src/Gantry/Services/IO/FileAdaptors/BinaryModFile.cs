using Gantry.Services.IO.Abstractions;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.DataStructures;
using Gantry.Services.IO.Extensions;
using Vintagestory.API.Util;

namespace Gantry.Services.IO.FileAdaptors;

/// <summary>
///     Represents a binary file, used by the mod. This class cannot be inherited.
/// </summary>
/// <seealso cref="ModFile" />
/// <seealso cref="IBinaryModFile" />
public sealed class BinaryModFile : ModFile, IBinaryModFile
{
    /// <summary>
    /// 	Initialises a new instance of the <see cref="BinaryModFile"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public BinaryModFile(string filePath) : base(filePath)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="BinaryModFile"/> class.
    /// </summary>
    /// <param name="fileInfo">The file information.</param>
    public BinaryModFile(FileInfo fileInfo) : base(fileInfo)
    {
    }

    /// <summary>
    ///     Gets the type of the file.
    /// </summary>
    /// <value>The type of the file.</value>
    public override ModFileFormat FileFormat 
        => ModFileFormat.Binary;

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public override TModel ParseAs<TModel>() 
        => SerializerUtil.Deserialize<TModel>(File.ReadAllBytes(ModFileInfo.FullName));

    /// <summary>
    /// parse as as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
    public override async Task<TModel> ParseAsAsync<TModel>() 
        => SerializerUtil.Deserialize<TModel>(await ModFileInfo.ReadAllBytesAsync());

    /// <summary>
    ///     Deserialises the specified file as a collection of a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <see cref="IEnumerable{TModel}" />, populated with data from this file.</returns>
    public override IEnumerable<TModel> ParseAsMany<TModel>() 
        => SerializerUtil.Deserialize<IEnumerable<TModel>>(File.ReadAllBytes(ModFileInfo.FullName));

    /// <summary>
    ///     Deserialises the specified file as a collection of a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <see cref="IEnumerable{TModel}" />, populated with data from this file.</returns>
    public override async Task<IEnumerable<TModel>> ParseAsManyAsync<TModel>() 
        => SerializerUtil.Deserialize<IEnumerable<TModel>>(await ModFileInfo.ReadAllBytesAsync());

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override void SaveFrom<TModel>(TModel instance) 
        => File.WriteAllBytes(ModFileInfo.FullName, [.. SerializerUtil.Serialize(instance)]);

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override void SaveFromList<TModel>(IEnumerable<TModel> instance) 
        => File.WriteAllBytes(ModFileInfo.FullName, [.. SerializerUtil.Serialize(instance)]);

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public override async Task SaveFromListAsync<TModel>(IEnumerable<TModel> instance) 
        => await Task.Factory.StartNew(() => SaveFromList(instance));

    /// <summary>
    /// Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    /// <returns>Task.</returns>
    public override async Task SaveFromAsync<TModel>(TModel instance) 
        => await ModFileInfo.WriteAllBytesAsync([.. SerializerUtil.Serialize(instance)]);

    /// <summary>
    /// Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    /// <returns>Task.</returns>
    public override async Task SaveFromAsync<TModel>(IEnumerable<TModel> collection) 
        => await ModFileInfo.WriteAllBytesAsync([.. SerializerUtil.Serialize(collection)]);

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    public override void SaveFrom<TModel>(IEnumerable<TModel> collection) 
        => File.WriteAllBytes(ModFileInfo.FullName, [.. SerializerUtil.Serialize(collection)]);

    /// <summary>
    ///     Parses the file into a primitive byte array.
    /// </summary>
    /// <returns>An array of type <see cref="byte" />, populated with data from this file.</returns>
    public byte[] ParseAsByteArray() 
        => File.ReadAllBytes(ModFileInfo.FullName);

    /// <summary>
    ///     Parses the file into a primitive byte array.
    /// </summary>
    /// <returns>An array of type <see cref="byte" />, populated with data from this file.</returns>
    public async Task<byte[]> ParseAsByteArrayAsync() 
        => await ModFileInfo.ReadAllBytesAsync();

    /// <summary>
    ///     Parses the file into a memory stream.
    /// </summary>
    /// <returns>An instance of type <see cref="MemoryStream" />, populated with data from this file.</returns>
    public MemoryStream ParseAsMemoryStream() 
        => new(ParseAsByteArray());

    /// <summary>
    ///     Parses the file into a memory stream.
    /// </summary>
    /// <returns>An instance of type <see cref="MemoryStream" />, populated with data from this file.</returns>
    public async Task<MemoryStream> ParseAsMemoryStreamAsync() 
        => await Task.Factory.StartNew(ParseAsMemoryStream);
}