using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace Gantry.Services.HarmonyPatches
{
    /// <summary>
    ///     Provides methods of applying Harmony patches to the game.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public interface IHarmonyPatchingService : IDisposable
    {
        /// <summary>
        ///     Apply patches to the game, located in the mod assembly.
        /// </summary>
        void PatchModAssembly();

        /// <summary>
        ///     Creates a new patch host, if one with the specified ID doesn't already exist.
        /// </summary>
        /// <param name="harmonyId">The identifier to use for the patch host.</param>
        /// <returns>A <see cref="Harmony" /> patch host.</returns>
        Harmony CreateOrUseInstance(string harmonyId);

        /// <summary>
        ///     Gets the default harmony instance for the mod.
        /// </summary>
        /// <value>The default Harmony instance for the mod, with the mod assembly's full name as the instance ID.</value>
        Harmony Default { get; }

        /// <summary>
        /// By default, all annotated [HarmonyPatch] classes in the executing assembly will
        /// be processed at launch. Manual patches can be processed later on at runtime.
        /// </summary>
        void PatchAssembly(Assembly assembly);
    }
}