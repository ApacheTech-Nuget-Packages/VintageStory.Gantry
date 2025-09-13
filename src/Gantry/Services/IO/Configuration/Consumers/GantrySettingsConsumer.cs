using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Configuration.Consumers;

/// <summary>
///     Represents a class that affects, or is affected by specific feature settings.
/// </summary>
/// <typeparam name="TSettings">The settings file to use within the patches in this class.</typeparam>
public abstract class GantrySettingsConsumer<TSettings>(ICoreGantryAPI core) :
    SettingsConsumerBase<TSettings>(ModFileScope.Gantry, core)
    where TSettings : FeatureSettings<TSettings>, new();