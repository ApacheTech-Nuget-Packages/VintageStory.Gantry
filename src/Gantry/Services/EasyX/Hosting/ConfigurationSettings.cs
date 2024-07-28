using Gantry.Core;
using Gantry.Services.FileSystem.Configuration.Abstractions;
using Gantry.Services.FileSystem.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gantry.Services.EasyX.Hosting;

/// <summary>
///     The settings used to configure the mod.
/// </summary>
[JsonObject]
public class ConfigurationSettings : FeatureSettings
{
    internal static ConfigurationSettings Instance { get; set; } = new();

    /// <summary>
    ///     Specifies whether to use world, or global settings to save feature settings to.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public FileScope Scope { get; set; } = FileScope.World;

    /// <summary>
    ///     The name of the command used to administrate the mod.
    /// </summary>
    public string CommandName { get; set; } = ModEx.ModInfo.ModID;
}