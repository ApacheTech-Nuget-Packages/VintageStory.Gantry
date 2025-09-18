using Gantry.Core.Abstractions;

namespace Gantry.Services.HarmonyPatches.Abstractions;

/// <summary>
///     Represents a base class for creating Harmony patches that do not require feature settings.
/// </summary>
public abstract class GantryPatch : IGantryPatchClass
{
    /// <inheritdoc />
    public virtual void Initialise(ICoreGantryAPI core)
        => Gantry = core;

    /// <summary>
    ///     The feature settings associated with this patch class.
    /// </summary>
    protected static ICoreGantryAPI Gantry { get; private set; } = default!;
}