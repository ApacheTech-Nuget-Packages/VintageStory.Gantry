using Gantry.Services.IO.Abstractions.Contracts;

namespace Gantry.Services.IO.Configuration.Abstractions;

/// <summary>
///     Represents a settings file for the mod, in JSON format.
/// </summary>
public interface IJsonSettingsFile : IDisposable
{
    /// <summary>
    ///     Gets the underlying <see cref="IJsonModFile"/> that this instance wraps.
    /// </summary>
    /// <value>
    ///     The file underlying JSON file from the file system.
    /// </value>
    IJsonModFile File { get; }

    /// <summary>
    ///     Whether the settings are for the client, or the server.
    /// </summary>
    /// <value>
    ///     The App Side the settings file runs on.
    /// </value>
    EnumAppSide Side { get; }

    /// <summary>
    ///     Retrieves the settings for a specific feature, parsed as a strongly-typed POCO class instance.
    ///     Changes made to the settings will automatically be written to the file, as they are set.
    /// </summary>
    /// <typeparam name="TSettings">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>An object, that represents the settings for a given mod feature.</returns>
    TSettings Feature<TSettings>(string? featureName = null) where TSettings : FeatureSettings<TSettings>, new();

    /// <summary>
    ///     Saves the specified settings to file.
    /// </summary>
    /// <typeparam name="TSettings">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="settings">The settings.</param>
    void Save<TSettings>(TSettings settings, string? featureName = null);
}