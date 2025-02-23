namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Provides extension methods for checking the mod loader status.
/// </summary>
public static class ModLoaderExtensions
{
    /// <summary>
    ///     Determines whether any of the specified mods are loaded.
    /// </summary>
    /// <param name="modLoader">The game's mod loader.</param>
    /// <param name="modIds">An array of mod identifiers to check.</param>
    /// <returns>
    ///     <c>true</c> if any of the specified mods are loaded; otherwise, <c>false</c>.
    /// </returns>
    public static bool AreAnyModsLoaded(this IModLoader modLoader, params string[] modIds)
    {
        foreach (var modId in modIds)
        {
            if (modLoader.IsModEnabled(modId)) return true;
        }
        return false;
    }

    /// <summary>
    ///     Determines whether all of the specified mods are loaded.
    /// </summary>
    /// <param name="modLoader">The game's mod loader.</param>
    /// <param name="modIds">An array of mod identifiers to check.</param>
    /// <returns>
    ///     <c>true</c> if all of the specified mods are loaded; otherwise, <c>false</c>.
    /// </returns>
    public static bool AreAllModsLoaded(this IModLoader modLoader, params string[] modIds)
    {
        foreach (var modId in modIds)
        {
            if (!modLoader.IsModEnabled(modId)) return false;
        }
        return true;
    }
}