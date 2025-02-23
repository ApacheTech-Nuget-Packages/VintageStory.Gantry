#nullable enable
using Gantry.Core.Extensions.DotNet;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Gantry.Core;

/// <summary>
///     Extended functionality for the <see cref="Lang"/> class.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class LangEx
{
    [SuppressMessage("Style", "IDE1006:Naming Styles")]
    private static readonly Dictionary<string, JsonObject> Translations = [];

    private static JsonObject GetTranslationsFile(string category = "Gantry", string? locale = null)
    {
        locale ??= Lang.CurrentLocale;
        try
        {
            if (Translations.TryGetValue(category, out var translations)) return translations;
            var json = typeof(LangEx).Assembly.GetResourceContent($"lang.{category}.{locale}.json");
            translations = JsonObject.FromJson(json);
            Translations.AddIfNotPresent(category, translations);
            return translations;
        }
        catch (JsonReaderException)
        {
            return GetTranslationsFile(locale: "en");
        }
    }

    /// <summary>
    ///     Returns a localised string based on the state of a boolean variable.
    /// </summary>
    /// <param name="state">The boolean value to localise.</param>
    /// <returns>A localised string representation of the boolean value.</returns>
    public static string BooleanString(bool state)
    {
        var translations = GetTranslationsFile("Gantry");
        return translations[$"boolean-{state}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="value">The value to localise.</param>
    /// <returns>A localised string representation of the boolean value.</returns>
    public static string ConfirmationString(string value)
    {
        var translations = GetTranslationsFile("Gantry");
        return translations[$"confirmation-{value}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the month value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the full name of the month of the year.</returns>
    public static string FullMonthString(DateTime dateTime)
    {
        var translations = GetTranslationsFile("Gantry");
        return translations[$"datetime-months-full-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the month value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the abbreviated name of the month of the year.</returns>
    public static string AbbreviatedMonthString(DateTime dateTime)
    {
        var translations = GetTranslationsFile("Gantry");
        return translations[$"datetime-months-abbr-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the day value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the full name of the day of the week.</returns>
    public static string FullDayString(DateTime dateTime)
    {
        var translations = GetTranslationsFile("Gantry");
        return translations[$"datetime-days-full-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the day value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string AbbreviatedDayString(DateTime dateTime)
    {
        var translations = GetTranslationsFile("Gantry");
        return translations[$"datetime-days-abbr-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string FeatureString(string featureName, string path, params object[] args) 
        => Lang.Get(FeatureCode(featureName, path), args);

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="culture">The language to get the string in.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string CultureString(string culture, string featureName, string path, params object[] args)
        => Lang.GetL(culture, FeatureCode(featureName, path), args);

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="domain">The mod domain the string belongs to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string FeatureStringFromDomain(string domain, string featureName, string path, params object[] args) 
        => Lang.Get(FeatureCode(domain, featureName, path), args);

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string FeatureCode(string featureName, string path) 
        => FeatureCode(ModEx.ModInfo.ModID, featureName, path);

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="domain">The mod domain the code belongs to.</param>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string FeatureCode(string domain, string featureName, string path) 
        => $"{domain}:Features.{featureName}.{path}";

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string EmbeddedFeatureString(string featureName, string path, params object[] args) 
        => GetGantryEmbedded(featureName, $"Features.{featureName}.{path}", args);

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="category">The category determines the embedded file to use for translations.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string from the current mod's language files.</returns>
    public static string GetGantryEmbedded(string category, string path, params object[] args)
    {
        var translations = GetTranslationsFile(category);
        return string.Format(translations[path].AsString(), args);
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <returns>A localised string from the current mod's language files.</returns>
    public static string Get(string path) 
        => Lang.Get($"{ModEx.ModInfo.ModID}:{path}");

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string from the current mod's language files.</returns>
    public static string Get(string path, params object[] args) 
        => Lang.Get($"{ModEx.ModInfo.ModID}:{path}", args);

    /// <summary>
    ///     Returns a string, based on whether the specified value if greater than one (1).
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <returns>A localised string from the mod's language files.</returns>
    public static string Pluralise(string path, int value)
    {
        var suffix = Math.Abs(value) == 1 ? "singular" : "plural";
        return Lang.Get($"{path}-{suffix}");
    }

    /// <summary>
    ///     Returns the title of the mod.
    /// </summary>
    /// <returns>A localised string from the mod's language files.</returns>
    public static string ModTitle() 
        => Get("ModTitle");

    /// <summary>
    ///     Returns the language code of the specified player.
    /// </summary>
    public static string GetPlayerLanguageCode(IPlayer serverPlayer) 
        => (serverPlayer as IServerPlayer)?.LanguageCode ?? Lang.CurrentLocale;
}