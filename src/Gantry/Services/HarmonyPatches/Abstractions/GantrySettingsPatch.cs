using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;

namespace Gantry.Services.HarmonyPatches.Abstractions;

/// <summary>
///     Represents a base class for creating Harmony patches that utilise feature settings.
/// </summary>
/// <typeparam name="T">The type of <see cref="FeatureSettings{T}"/> associated with the patch.</typeparam>
public abstract class GantrySettingsPatch<T> : GantryPatch, IGantryPatchClass
    where T : FeatureSettings<T>, new()
{
    /// <inheritdoc />
    public override void Initialise(ICoreGantryAPI core)
    {
        base.Initialise(core);
        Settings = core.Services.GetRequiredService<T>();
    }

    /// <summary>
    ///     The feature settings associated with this patch class.
    /// </summary>
    protected static T Settings { get; private set; } = default!;
}