using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Configuration.Consumers;

/// <summary>
///     Represents a class that affects, or is affected by specific feature settings.
/// </summary>
public abstract class SettingsConsumerBase<TSettings> : ISettingsConsumer 
    where TSettings : FeatureSettings<TSettings>, new()
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="SettingsConsumerBase{TSettings}"/> class.
    /// </summary>
    /// <param name="scope">The scope of the settings file to use (gantry, world, or global).</param>
    /// <param name="core">The core Gantry API instance, provided by the mod.</param>
    protected SettingsConsumerBase(ModFileScope scope, ICoreGantryAPI core)
    {
        Core = core;
        Scope = scope;
        var system = core.Settings;
        var settingsFile = system.For(scope);
        Settings = settingsFile.Feature<TSettings>();
    }

    /// <summary>
    ///     The name of the feature associated with the settings.
    /// </summary>
    protected string FeatureName => typeof(TSettings).Name.Replace("Settings", "");

    /// <summary>
    ///     The scope of the settings file to use (gantry, world, or global).
    /// </summary>
    protected ModFileScope Scope { get; private set; }

    /// <summary>
    ///     The Gantry Core API for the current mod and app side.
    /// </summary>
    protected ICoreGantryAPI Core { get; private set; } = default!;

    /// <summary>
    ///     The settings file to use within the patches in this class.
    /// </summary>
    protected static TSettings Settings { get; private set; } = default!;

    /// <summary>
    ///     Saves any changes to the mod settings file.
    /// </summary>
    /// <exception cref="UnreachableException"></exception>
    protected void SaveChanges()
    {
        switch (Scope)
        {
            case ModFileScope.Global:
                Core.Settings.Global.Save(Settings, FeatureName);
                break;
            case ModFileScope.World:
                Core.Settings.World.Save(Settings, FeatureName);
                break;
            case ModFileScope.Gantry:
                Core.Settings.Gantry.Save(Settings, FeatureName);
                break;
            default:
                throw new UnreachableException();
        }
    }
}
