using System.Reflection;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.Abstractions.Contracts;

/// <summary>
///     Represents a file on the filesystem.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IModFile : IModFileBase
{
    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel"/>, populated with data from this file.</returns>
    public TModel ParseAs<TModel>()
        where TModel : class, new();

    /// <summary>
    ///     Deserialises the specified file as a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <typeparamref name="TModel"/>, populated with data from this file.</returns>
    public Task<TModel> ParseAsAsync<TModel>()
        where TModel : class, new();

    /// <summary>
    ///     Deserialises the specified file as a collection of a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <see cref="IEnumerable{TModel}"/>, populated with data from this file.</returns>
    public IEnumerable<TModel> ParseAsMany<TModel>()
        where TModel : class, new();

    /// <summary>
    ///     Deserialises the specified file as a collection of a strongly-typed object.
    ///     The consuming type must have a paramaterless constructor.
    /// </summary>
    /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
    /// <returns>An instance of type <see cref="IEnumerable{TModel}"/>, populated with data from this file.</returns>
    public Task<IEnumerable<TModel>> ParseAsManyAsync<TModel>()
        where TModel : class, new();

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public void SaveFrom<TModel>(TModel instance)
        where TModel : class, new();

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public void SaveFromList<TModel>(IEnumerable<TModel> instance)
        where TModel : class, new();

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public Task SaveFromListAsync<TModel>(IEnumerable<TModel> instance)
        where TModel : class, new();

    /// <summary>
    ///     Serialises the specified instance, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="instance">The instance of the object to serialise.</param>
    public Task SaveFromAsync<TModel>(TModel instance)
        where TModel : class, new();

    /// <summary>
    ///     Serialises the specified collection of objects, and saves the resulting data to file.
    /// </summary>
    /// <typeparam name="TModel">The type of the object to serialise.</typeparam>
    /// <param name="collection">The collection of the objects to save to a single file.</param>
    public Task SaveFromAsync<TModel>(IEnumerable<TModel> collection)
        where TModel : class, new();

    /// <summary>
    ///     Disembeds the file from a specific assembly.
    /// </summary>
    /// <param name="assembly">The assembly to disembed the file from.</param>
    public void DisembedFrom(Assembly assembly);

    /// <summary>
    ///     Disembeds the file from the mod assembly.
    /// </summary>
    /// <param name="filePath">The absolute path, on the local system, to disembed the file to.</param>
    public void Disembed(string filePath);

    /// <summary>
    ///     Disembeds the file from the mod assembly, to the default file location.
    /// </summary>
    public void Disembed();
}