using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;

namespace Gantry.Services.IO.Configuration.Consumers;

/// <summary>
///     Represents a class that affects, or is affected by specific feature settings.
/// </summary>
/// <typeparam name="TSettings">The settings file to use within the patches in this class.</typeparam>
public abstract class WorldSettingsConsumer<TSettings> : ISettingsConsumer where TSettings : FeatureSettings<TSettings>, new()
{
    private static TSettings? _settings;
    private static ICoreGantryAPI _core = default!;

    /// <summary>
    ///     Initialises a new instance of the <see cref="WorldSettingsConsumer{TSettings}"/> class.
    /// </summary>
    /// <param name="core">The core Gantry API instance, provided by the mod.</param>
    public WorldSettingsConsumer(ICoreGantryAPI core)
    {
        _core = core;
    }

    /// <summary>
    ///     Gets or sets the settings.
    /// </summary>
    /// <value>
    ///     The settings.
    /// </value>
    protected static TSettings? Settings => _settings ??= _core.Settings.World.Feature<TSettings>();

    /// <summary>
    ///     Gets or sets the name of the feature.
    /// </summary>
    /// <value>
    ///     The name of the feature.
    /// </value>
    protected static string FeatureName => typeof(TSettings).Name.Replace("Settings", "");

    /// <summary>
    ///     Saves any changes to the mod settings file.
    /// </summary>
    protected void SaveChanges()
    {
        _core.Settings.World.Save(Settings, FeatureName);
    }
}