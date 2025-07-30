namespace Gantry.Core.Hosting;

/// <summary>
///     Represents configuration options for the Gantry host.
/// </summary>
public class GantryHostOptions
{
    /// <summary>
    ///     The default configuration options for the Gantry host.
    /// </summary>
    public static GantryHostOptions Default { get; } = new();

    /// <summary>
    ///     Indicates whether debug mode is enabled.
    /// </summary>
    public bool DebugMode { get; set; } = false;
    
    /// <summary>
    ///     Indicates whether telemetry is enabled.
    /// </summary>
    public bool EnableTelemetry { get; set; } = true;

    /// <summary>
    ///     Indicates whether patches should be applied.
    /// </summary>
    public bool ApplyPatches { get; set; } = true;

    /// <summary>
    ///     Indicates whether settings files should be registered.
    /// </summary>
    public bool RegisterSettingsFiles { get; set; } = true;
}
