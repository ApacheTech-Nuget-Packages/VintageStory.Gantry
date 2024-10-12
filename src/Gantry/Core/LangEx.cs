#nullable enable
using ApacheTech.Common.Extensions.System;
using Gantry.Core.Extensions.DotNet;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

namespace Gantry.Core;

/// <summary>
///     Extended functionality for the <see cref="Lang"/> class.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class LangEx
{
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
    {
        return Lang.Get(FeatureCode(featureName, path), args);
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string FeatureCode(string featureName, string path)
    {
        return $"{ModEx.ModInfo.ModID}:Features.{featureName}.{path}";
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="featureName">The name of the feature.</param>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string EmbeddedFeatureString(string featureName, string path, params object[] args)
    {
        return GetGantryEmbedded(featureName, $"Features.{featureName}.{path}", args);
    }

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
    {
        return Lang.Get($"{ModEx.ModInfo.ModID}:{path}");
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    /// <returns>A localised string from the current mod's language files.</returns>
    public static string Get(string path, params object[] args)
    {
        return Lang.Get($"{ModEx.ModInfo.ModID}:{path}", args);
    }

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
    {
        return Get("ModTitle");
    }
}