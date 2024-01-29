using Gantry.Services.FileSystem.Abstractions.Contracts;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Gantry.Services.FileSystem.Configuration.Abstractions;

/// <summary>
///     Represents a settings file for the mod, in JSON format.
/// </summary>
public interface IJsonSettingsFile
{
    /// <summary>
    ///     Gets the underlying <see cref="IJsonModFile"/> that this instance wraps.
    /// </summary>
    /// <value>The file underlying JSON file from the file system.</value>
    IJsonModFile File { get; }

    /// <summary>
    ///     Retrieves the settings for a specific feature, parsed as a strongly-typed POCO class instance.
    ///     Changes made to the settings will automatically be written to the file, as they are set.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>An object, that represents the settings for a given mod feature.</returns>
    T Feature<T>(string featureName = null) where T : class, new();

    /// <summary>
    ///     Saves the specified settings to file.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="settings">The settings.</param>
    void Save<T>(T settings, string featureName = null);

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void Dispose();
}