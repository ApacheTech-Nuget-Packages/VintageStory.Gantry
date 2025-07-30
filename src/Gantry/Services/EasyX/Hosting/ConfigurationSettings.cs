using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;
using Newtonsoft.Json.Converters;

namespace Gantry.Services.EasyX.Hosting;

/// <summary>
///     The settings used to configure the mod.
/// </summary>
[JsonObject]
public class ConfigurationSettings() : FeatureSettings<ConfigurationSettings>
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="ConfigurationSettings"/> class.
    /// </summary>
    /// <param name="commandName"></param>
    public ConfigurationSettings(string commandName) : this()
    {
        CommandName = commandName;
    }

    /// <summary>
    ///     Specifies whether to use world, or global settings to save feature settings to.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public ModFileScope Scope { get; set; } = ModFileScope.World;

    /// <summary>
    ///     The name of the command used to administrate the mod.
    /// </summary>
    public string CommandName { get; set; } = string.Empty;
}