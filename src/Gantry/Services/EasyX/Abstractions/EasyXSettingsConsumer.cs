using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.Configuration.Consumers;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Represents a class that affects, or is affected by specific feature settings.
/// </summary>
public abstract class EasyXSettingsConsumer<TSettings> : SettingsConsumerBase<TSettings>, ISettingsConsumer
    where TSettings : FeatureSettings<TSettings>, new ()
{
    private readonly ConfigurationSettings _configuration;

    /// <summary>
    ///     Initialises a new instance of the <see cref="SettingsConsumerBase{TSettings}"/> class.
    /// </summary>
    /// <param name="core">The core Gantry API instance, provided by the mod.</param>
    protected EasyXSettingsConsumer(ICoreGantryAPI core) : base(ModFileScope.World, core)
    {
        _configuration = core.Settings.Global.Feature<ConfigurationSettings>();
    }

    /// <summary>
    ///     The scope of the settings file to use (gantry, world, or global).
    /// </summary>
    protected new ModFileScope Scope => _configuration.Scope;

    /// <summary>
    ///     The settings file to use within the patches in this class.
    /// </summary>
    protected new TSettings Settings => Core.Services.GetRequiredService<TSettings>();
}
