using ProtoBuf;
using System.ComponentModel;

namespace Gantry.Services.EasyX;

/// <summary>
///     A packet to indicate that settings have been saved.
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class SettingsSavedPacket
{
    /// <summary>
    ///     The name of the feature whose settings have been saved.
    /// </summary>
    [DefaultValue("")]
    public string FeatureName { get; set; } = string.Empty;
}