using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;
using Newtonsoft.Json.Converters;
using ProtoBuf;
using System.ComponentModel;

namespace Gantry.Services.EasyX;

/// <summary>
///     The settings used to configure the mod.
/// </summary>
[JsonObject]
[ProtoContract]
public class ConfigurationSettings() : FeatureSettings<ConfigurationSettings>
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="ConfigurationSettings"/> class.
    /// </summary>
    /// <param name="commandName">The name of the command used to administrate the mod.</param>
    public ConfigurationSettings(string commandName) : this()
    {
        CommandName = commandName;
    }

    /// <summary>
    ///     Specifies whether to use world, or global settings to save feature settings to.
    /// </summary>
    [ProtoMember(1)]
    [DefaultValue(ModFileScope.World)]
    [JsonConverter(typeof(StringEnumConverter))]
    public ModFileScope Scope { get; set; } = ModFileScope.World;

    /// <summary>
    ///     The name of the command used to administrate the mod.
    /// </summary>
    [ProtoMember(3)]
    [DefaultValue("")]
    public string CommandName { get; set; } = string.Empty;
}