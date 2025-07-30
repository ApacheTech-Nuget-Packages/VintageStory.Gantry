using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Configuration;

/// <summary>
///     Represents a service for managing mod settings files.
/// </summary>
public interface IModSettingsService : IDisposable
{
    /// <summary>
    ///     The global mod settings; these settings will persist through each gameworld.
    /// </summary>
    /// <value>The global settings.</value>
    IJsonSettingsFile Global { get; }

    /// <summary>
    ///     The per-world mod settings; these settings can change within each gameworld.
    /// </summary>
    /// <value>The per-world settings.</value>
    IJsonSettingsFile World { get; }

    /// <summary>
    ///     The gantry core mod settings; these settings will persist through each gameworld, and be shared between mods.
    /// </summary>
    /// <value>The per-world settings.</value>
    IJsonSettingsFile Gantry { get; }

    /// <summary>
    ///     The mod settings for a specific <see cref="ModFileScope"/>.
    /// </summary>
    /// <param name="scope">The scope to retrieve the settings for.</param>
    IJsonSettingsFile For(ModFileScope scope);

    /// <summary>
    ///     Copies the settings from one scope to another, overwriting the destination scope's settings.
    /// </summary>
    /// <param name="fromScope">The scope to copy the settings from.</param>
    /// <param name="toScope">The scope to copy the settings to.</param>
    void CopySettings(ModFileScope fromScope, ModFileScope toScope);

    /// <summary>
    ///     Sets the settings for a specific <see cref="ModFileScope"/>.
    /// </summary>
    /// <param name="side">The side for which to set the settings.</param>
    /// <param name="scope">The scope to set the settings for.</param>
    /// <param name="settings">The settings to set for the specified scope.</param>
    internal void Set(EnumAppSide side, ModFileScope scope, IJsonSettingsFile settings);
}
