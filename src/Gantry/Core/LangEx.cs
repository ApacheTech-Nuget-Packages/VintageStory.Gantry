using Gantry.Core.Extensions.DotNet;
using JetBrains.Annotations;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

namespace Gantry.Core;

/// <summary>
///     Extended functionality for the <see cref="Lang"/> class.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class LangEx
{
    private static readonly JsonObject Translations;

    /// <summary>
    ///     Initialises the <see cref="LangEx"/> class.
    /// </summary>
    static LangEx()
    {
        var json = typeof(LangEx).Assembly.GetResourceContent("en.json");
        try
        {
            json = typeof(LangEx).Assembly.GetResourceContent($"{Lang.CurrentLocale}.json");
        }
        finally
        {
            Translations = JsonObject.FromJson(json);
        }
    }

    /// <summary>
    ///     Returns a localised string based on the state of a boolean variable.
    /// </summary>
    /// <param name="state">The boolean value to localise.</param>
    /// <returns>A localised string representation of the boolean value.</returns>
    public static string BooleanString(bool state)
    {
        return Translations[$"boolean-{state}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string.
    /// </summary>
    /// <param name="value">The value to localise.</param>
    /// <returns>A localised string representation of the boolean value.</returns>
    public static string ConfirmationString(string value)
    {
        return Translations[$"confirmation-{value}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the month value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the full name of the month of the year.</returns>
    public static string FullMonthString(DateTime dateTime)
    {
        return Translations[$"datetime-months-full-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the month value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the abbreviated name of the month of the year.</returns>
    public static string AbbreviatedMonthString(DateTime dateTime)
    {
        return Translations[$"datetime-months-abbr-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the day value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the full name of the day of the week.</returns>
    public static string FullDayString(DateTime dateTime)
    {
        return Translations[$"datetime-days-full-{dateTime.Month}"].AsString();
    }

    /// <summary>
    ///     Returns a localised string based on the day value within a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> object to localise.</param>
    /// <returns>A localised string representation of the abbreviated name of the day of the week.</returns>
    public static string AbbreviatedDayString(DateTime dateTime)
    {
        return Translations[$"datetime-days-abbr-{dateTime.Month}"].AsString();
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