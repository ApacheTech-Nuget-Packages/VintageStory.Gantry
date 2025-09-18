using Gantry.Core.Abstractions;

namespace Gantry.Services.HarmonyPatches.Abstractions;

/// <summary>
///     Represents a class that contains Harmony patches to be applied to the game.
/// </summary>
public interface IGantryPatchClass
{
    /// <summary>
    ///     Initialises the patch class with the provided <see cref="ICoreGantryAPI"/> instance.
    /// </summary>
    /// <param name="core">The core Gantry API instance.</param>
    void Initialise(ICoreGantryAPI core);
}