using Vintagestory.API.Common;

namespace Gantry.Services.FileSystem.Configuration.Consumers;

/// <summary>
///     Designates which app-side the settings are stored on.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SettingsConsumerAttribute : Attribute
{
    /// <summary>
    ///     Designates which app-side the settings are stored on.
    /// </summary>
    public EnumAppSide Side { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="SettingsConsumerAttribute"/> class.
    /// </summary>
    /// <param name="side">The side.</param>
    public SettingsConsumerAttribute(EnumAppSide side)
    {
        Side = side;
    }
}