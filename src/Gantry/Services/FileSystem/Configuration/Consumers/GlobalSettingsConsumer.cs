namespace Gantry.Services.FileSystem.Configuration.Consumers;

/// <summary>
///     Represents a class that affects, or is affected by specific feature settings.
/// </summary>
/// <typeparam name="TSettings">The settings file to use within the patches in this class.</typeparam>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class GlobalSettingsConsumer<TSettings> : ISettingsConsumer where TSettings : FeatureSettings<TSettings>, new()
{
    private static TSettings _settings;
    
    /// <summary>
    ///     Gets or sets the settings.
    /// </summary>
    /// <value>
    ///     The settings.
    /// </value>
    protected internal static TSettings Settings => _settings ??= ModSettings.Global?.Feature<TSettings>();

    /// <summary>
    ///     Gets or sets the name of the feature.
    /// </summary>
    /// <value>
    ///     The name of the feature.
    /// </value>
    protected internal static string FeatureName => typeof(TSettings).Name.Replace("Settings", "");

    /// <summary>
    ///     Saves any changes to the mod settings file.
    /// </summary>
    protected void SaveChanges()
    {
        ModSettings.Global.Save(Settings, FeatureName);
    }
}