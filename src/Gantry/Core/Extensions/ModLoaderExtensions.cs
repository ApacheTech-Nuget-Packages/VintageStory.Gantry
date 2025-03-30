using Vintagestory.Common;

namespace Gantry.Core.Extensions;

/// <summary>
///     Extension methods to aid with core game engine tasks.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ModLoaderExtensions
{
    /// <summary>
    ///     Returns a loaded <see cref="ModSystem"/> with the specified type.
    /// </summary>
    /// <param name="modLoader">The instance used to load mods.</param>
    /// <param name="type">The type of <see cref="ModSystem"/> to find.</param>
    /// <returns></returns>
    public static ModSystem? GetModSystem(this ModLoader modLoader, Type type)
    {
        return modLoader.Systems.FirstOrDefault(p => p.GetType() == type);
    }

    /// <summary>
    ///     Returns a loaded <see cref="ModSystem"/> with the specified type.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ModSystem"/> to find.</typeparam>
    /// <param name="modLoader">The instance used to load mods.</param>
    /// <returns></returns>
    public static T? GetModSystem<T>(this ModLoader modLoader) where T : ModSystem
    {
        return (T?)modLoader.Systems.FirstOrDefault(p => p.GetType() == typeof(T));
    }
}