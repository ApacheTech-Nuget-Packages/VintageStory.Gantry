using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gantry.Services.FileSystem.Abstractions;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;
using Gantry.Services.FileSystem.Extensions;
using JetBrains.Annotations;
using Vintagestory.API.Util;

namespace Gantry.Services.FileSystem.FileAdaptors
{
    /// <summary>
    ///     Represents a binary file, used by the mod. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ModFile" />
    /// <seealso cref="IBinaryModFile" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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
        public override FileType FileType => FileType.Binary;

        /// <summary>
        ///     Deserialises the specified file as a strongly-typed object.
        ///     The consuming type must have a paramaterless constructor.
        /// </summary>
        /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
        /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
        public override TModel ParseAs<TModel>()
        {
            var bytes = File.ReadAllBytes(ModFileInfo.FullName);
            return SerializerUtil.Deserialize<TModel>(bytes);
        }

        /// <summary>
        /// parse as as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
        /// <returns>An instance of type <typeparamref name="TModel" />, populated with data from this file.</returns>
        public override async Task<TModel> ParseAsAsync<TModel>()
        {
            var bytes = await ModFileInfo.ReadAllBytesAsync();
            return SerializerUtil.Deserialize<TModel>(bytes);
        }

        /// <summary>
        ///     Deserialises the specified file as a collection of a strongly-typed object.
        ///     The consuming type must have a paramaterless constructor.
        /// </summary>
        /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
        /// <returns>An instance of type <see cref="IEnumerable{TModel}" />, populated with data from this file.</returns>
        public override IEnumerable<TModel> ParseAsMany<TModel>()
        {
            var bytes = File.ReadAllBytes(ModFileInfo.FullName);
            return SerializerUtil.Deserialize<IEnumerable<TModel>>(bytes);
        }

        /// <summary>
        ///     Deserialises the specified file as a collection of a strongly-typed object.
        ///     The consuming type must have a paramaterless constructor.
        /// </summary>
        /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
        /// <returns>An instance of type <see cref="IEnumerable{TModel}" />, populated with data from this file.</returns>
        public override async Task<IEnumerable<TModel>> ParseAsManyAsync<TModel>()
        {
            var bytes = await ModFileInfo.ReadAllBytesAsync();
            return SerializerUtil.Deserialize<IEnumerable<TModel>>(bytes);
        }

        /// <summary>
        ///     Serialises the specified instance, and saves the resulting data to file.
        /// </summary>
        /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
        /// <param name="instance">The instance of the object to serialise.</param>
        public override void SaveFrom<TModel>(TModel instance)
        {
            File.WriteAllBytes(ModFileInfo.FullName, SerializerUtil.Serialize(instance).ToArray());
        }

        /// <summary>
        /// Serialises the specified instance, and saves the resulting data to file.
        /// </summary>
        /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
        /// <param name="instance">The instance of the object to serialise.</param>
        /// <returns>Task.</returns>
        public override Task SaveFromAsync<TModel>(TModel instance)
        {
            return ModFileInfo.WriteAllBytesAsync(SerializerUtil.Serialize(instance).ToArray());
        }

        /// <summary>
        /// Serialises the specified collection of objects, and saves the resulting data to file.
        /// </summary>
        /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
        /// <param name="collection">The collection of the objects to save to a single file.</param>
        /// <returns>Task.</returns>
        public override Task SaveFromAsync<TModel>(IEnumerable<TModel> collection)
        {
            return ModFileInfo.WriteAllBytesAsync(SerializerUtil.Serialize(collection).ToArray());
        }

        /// <summary>
        ///     Serialises the specified collection of objects, and saves the resulting data to file.
        /// </summary>
        /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
        /// <param name="collection">The collection of the objects to save to a single file.</param>
        public override void SaveFrom<TModel>(IEnumerable<TModel> collection)
        {
            File.WriteAllBytes(ModFileInfo.FullName, SerializerUtil.Serialize(collection).ToArray());
        }

        /// <summary>
        ///     Parses the file into a primitive byte array.
        /// </summary>
        /// <returns>An array of type <see cref="byte" />, populated with data from this file.</returns>
        public byte[] ParseAsByteArray()
        {
            return File.ReadAllBytes(ModFileInfo.FullName);
        }

        /// <summary>
        ///     Parses the file into a primitive byte array.
        /// </summary>
        /// <returns>An array of type <see cref="byte" />, populated with data from this file.</returns>
        public Task<byte[]> ParseAsByteArrayAsync()
        {
            return ModFileInfo.ReadAllBytesAsync();
        }

        /// <summary>
        /// Parses the file into a memory stream.
        /// </summary>
        /// <returns>An instance of type <see cref="MemoryStream" />, populated with data from this file.</returns>
        public MemoryStream ParseAsMemoryStream()
        {
            return new MemoryStream(ParseAsByteArray());
        }

        /// <summary>
        /// Parses the file into a memory stream.
        /// </summary>
        /// <returns>An instance of type <see cref="MemoryStream" />, populated with data from this file.</returns>
        public Task<MemoryStream> ParseAsMemoryStreamAsync()
        {
            return Task.Factory.StartNew(ParseAsMemoryStream);
        }
    }
}