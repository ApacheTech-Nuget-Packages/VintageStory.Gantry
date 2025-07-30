using System.ComponentModel;

namespace Gantry.Services.IO.DataStructures;

/// <summary>
///     Specifies the scope of a file within a mod.
/// </summary>
public enum ModFileScope
{
    /// <summary>
    ///     The file is specific to a single world.
    /// </summary>
    [Description("Per World")]
    World,

    /// <summary>
    ///     The file is specific to a single mod, but is available within all worlds.
    /// </summary>
    [Description("Global")]
    Global,

    /// <summary>
    ///     The file is shared across all Gantrified mods, at a global scope.
    /// </summary>
    [Description("Gantry")]
    Gantry
}