using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vintagestory.API.Datastructures;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.FileSystem.Abstractions.Contracts
{
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
        /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
        /// <param name="collection">The collection of the objects to save to a single file.</param>
        /// <param name="formatting">The JSON formatting style to use when serialising the data.</param>
        public void SaveFrom<TModel>(IEnumerable<TModel> collection, Formatting formatting = Formatting.Indented);

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
}