using Gantry.Services.FileSystem.v2.Abstractions;
using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.FileTypes.Json
{
    /// <summary>
    ///     Extension methods to aid providing JSON files to the user.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class JsonFileProviderExtensions
    {
        /// <summary>
        ///     Gets a JSON file, previously registered with the file provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="scope">The scope of the file.</param>
        /// <returns>A <see cref="IJsonFile"/> representation of the file, on the file system.</returns>
        public static IJsonFile GetJsonFile(this IFileProvider provider, string fileName, FileScope scope)
            => provider.Wrap<IJsonFile>(fileName, scope);
    }
}