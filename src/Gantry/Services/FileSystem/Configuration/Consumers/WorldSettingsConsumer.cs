namespace Gantry.Services.FileSystem.Configuration.Consumers;

/// <summary>
///     Represents a class that affects, or is affected by specific feature settings.
/// </summary>
/// <typeparam name="TSettings">The settings file to use within the patches in this class.</typeparam>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class WorldSettingsConsumer<TSettings> : ISettingsConsumer where TSettings : FeatureSettings<TSettings>, new()
{
    private static TSettings _settings;

    /// <summary>
    ///     Gets or sets the settings.
    /// </summary>
    /// <value>
    ///     The settings.
    /// </value>
    protected static TSettings Settings => _settings ??= ModSettings.World?.Feature<TSettings>();

    /// <summary>
    ///     Gets or sets the settings.
    /// </summary>
    /// <value>
    ///     The settings.
    /// </value>
    protected static TSettings ClientSettings => ModSettings.ClientWorld?.Feature<TSettings>();

    /// <summary>
    ///     Gets or sets the settings.
    /// </summary>
    /// <value>
    ///     The settings.
    /// </value>
    protected static TSettings ServerSettings => ModSettings.ServerWorld?.Feature<TSettings>();

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
        ModSettings.World.Save(Settings, FeatureName);
    }
}