using Vintagestory.API.Server;

namespace Gantry.Core;

/// <summary>
///     Provides extension methods for the <see cref="Lang"/> class, enabling advanced localisation and translation features for Gantry mods.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class LangEx
{
    #region UIUX Strings

    /// <summary>
    ///     Returns a localised string representation of a boolean value.
    /// </summary>
    /// <param name="state">The boolean value to localise.</param>
    /// <returns>The localised string for the boolean value.</returns>
    public static string Boolean(bool state) 
        => Get("gantry", $"boolean-{state}");

    /// <summary>
    ///     Returns a localised confirmation string for a given value.
    /// </summary>
    /// <param name="value">The value to localise as a confirmation.</param>
    /// <returns>The localised confirmation string.</returns>
    public static string Confirmation(string value) 
        => Get("gantry", $"confirmation-{value}");

    /// <summary>
    ///     Returns a localised month name for the specified <see cref="DateTime"/>.
    /// </summary>
    /// <param name="date">The date whose month to localise.</param>
    /// <param name="abbreviated">Whether to return the abbreviated month name.</param>
    /// <returns>The localised month name.</returns>
    public static string Month(DateTime date, bool abbreviated = false) 
        => Get("gantry", $"datetime-months-{(abbreviated ? "abbr" : "full")}-{date.Month}");

    /// <summary>
    ///     Returns a localised day-of-week name for the specified <see cref="DateTime"/>.
    /// </summary>
    /// <param name="date">The date whose day of week to localise.</param>
    /// <param name="abbreviated">Whether to return the abbreviated day name.</param>
    /// <returns>The localised day-of-week name.</returns>
    public static string Day(DateTime date, bool abbreviated = false) 
        => Get("gantry", $"datetime-days-{(abbreviated ? "abbr" : "full")}-{date.Month}");

    #endregion

    #region Feature Strings and Keys

    /// <summary>
    ///     Returns the translation key for a feature-specific path in the current mod domain.
    /// </summary>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <returns>The translation key string.</returns>
    public static string FeatureKey(string feature, string path)
        => FeatureKey(ModEx.ModInfo.ModID, feature, path);

    /// <summary>
    ///     Returns the translation key for a feature-specific path in a given domain.
    /// </summary>
    /// <param name="domain">The mod domain.</param>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <returns>The translation key string.</returns>
    public static string FeatureKey(string domain, string feature, string path) 
        => $"{domain}:Features.{feature}.{path}";

    /// <summary>
    ///     Returns a localised string for a feature-specific path in the current mod domain.
    /// </summary>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <param name="args">Optional arguments for formatting.</param>
    /// <returns>The localised string for the feature path.</returns>
    public static string FeatureString(string feature, string path, params object?[]? args) 
        => Lang.Get(FeatureKey(feature, path), args);

    /// <summary>
    ///     Returns a localised string for a feature-specific path in a specific domain.
    /// </summary>
    /// <param name="domain">The mod domain.</param>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <param name="args">Optional arguments for formatting.</param>
    /// <returns>The localised string for the feature path in the specified domain.</returns>
    public static string FeatureStringD(string domain, string feature, string path, params object?[]? args)
        => Lang.Get(FeatureKey(domain, feature, path), args);

    /// <summary>
    ///     Returns a localised string for a feature-specific path the Gantry domain.
    /// </summary>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <param name="args">Optional arguments for formatting.</param>
    /// <returns>The localised string for the feature path in the specified domain.</returns>
    public static string FeatureStringG(string feature, string path, params object?[]? args)
        => Lang.Get(FeatureKey("gantry", feature, path), args);

    /// <summary>
    ///     Returns a localised string for a feature-specific path in a specific culture.
    /// </summary>
    /// <param name="culture">The language/culture key.</param>
    /// <param name="feature">The feature name.</param>
    /// <param name="path">The translation path within the feature.</param>
    /// <param name="args">Arguments for formatting.</param>
    /// <returns>The localised string for the feature path in the specified culture.</returns>
    public static string FeatureStringL(string culture, string feature, string path, params object[] args) 
        => Lang.GetL(culture, FeatureKey(feature, path), args);

    #endregion

    #region General Get Methods

    /// <summary>
    ///     Returns a localised string from the current mod domain for the specified path.
    /// </summary>
    /// <param name="path">The translation path within the current mod domain.</param>
    /// <returns>The localised string from the current mod domain.</returns>
    public static string Get(string path)
        => Lang.Get($"{ModEx.ModInfo.ModID}:{path}");

    /// <summary>
    ///     Returns a localised string from the current mod domain for the specified path, with arguments.
    /// </summary>
    /// <param name="path">The translation path within the current mod domain.</param>
    /// <param name="args">Arguments for formatting.</param>
    /// <returns>The localised string from the current mod domain.</returns>
    public static string Get(string path, params object[] args) 
        => Lang.Get($"{ModEx.ModInfo.ModID}:{path}", args);

    /// <summary>
    ///     Returns a localised string from a specific domain for the specified path.
    /// </summary>
    /// <param name="domain">The mod domain.</param>
    /// <param name="path">The translation path within the domain.</param>
    /// <returns>The localised string from the specified domain.</returns>
    public static string GetD(string domain, string path) 
        => Lang.Get($"{domain}:{path}");

    /// <summary>
    ///     Returns a localised string from a specific domain for the specified path, with arguments.
    /// </summary>
    /// <param name="domain">The mod domain.</param>
    /// <param name="path">The translation path within the domain.</param>
    /// <param name="args">Arguments for formatting.</param>
    /// <returns>The localised string from the specified domain.</returns>
    public static string GetD(string domain, string path, params object[] args) 
        => Lang.Get($"{domain}:{path}", args);

    #endregion

    #region Gantry Domain Shortcuts

    /// <summary>
    ///     Returns a localised string from the gantry domain for the specified path.
    /// </summary>
    /// <param name="path">The translation path within the gantry domain.</param>
    /// <returns>The localised string from the gantry domain.</returns>
    public static string Gantry(string path) 
        => GetD("gantry", path);

    /// <summary>
    ///     Returns a localised string from the gantry domain for the specified path, with arguments.
    /// </summary>
    /// <param name="path">The translation path within the gantry domain.</param>
    /// <param name="args">Arguments for formatting.</param>
    /// <returns>The localised string from the gantry domain.</returns>
    public static string Gantry(string path, params object[] args) 
        => GetD("gantry", path, args);

    #endregion

    #region Pluralisation

    /// <summary>
    ///     Returns a localised pluralised string for the specified value and path.
    /// </summary>
    /// <param name="path">The translation path.</param>
    /// <param name="value">The value to determine singular/plural.</param>
    /// <returns>The localised pluralised string.</returns>
    public static string Pluralise(string path, int value) 
        => Lang.Get($"{path}-{(Math.Abs(value) == 1 ? "singular" : "plural")}");

    #endregion

    #region Mod Info

    /// <summary>
    ///     Returns the localised mod title.
    /// </summary>
    /// <returns>The localised mod title string.</returns>
    public static string ModTitle() 
        => Get("ModTitle");

    /// <summary>
    ///     Returns the localised mod description.
    /// </summary>
    /// <returns>The localised mod tidescriptiontle string.</returns>
    public static string ModDescription()
        => Get("ModDescription");

    #endregion

    #region Player

    /// <summary>
    ///     Returns the language key of the specified player, or the current locale if unavailable.
    /// </summary>
    /// <param name="player">The player to get the language key for.</param>
    /// <returns>The language key string.</returns>
    public static string PlayerLanguage(IPlayer player)
        => (player as IServerPlayer)?.LanguageCode ?? Lang.CurrentLocale;

    #endregion
}