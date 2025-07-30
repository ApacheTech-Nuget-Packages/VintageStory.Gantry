namespace Gantry.Tools.TranslationsPatcher.Properties;

/// <summary>
///     Provides constant values for translation folder names and required culture.
/// </summary>
public static class Constants
{
    /// <summary>
    ///     The name of the translations folder (default: "_Translations").
    /// </summary>
    public const string TranslationsFolder = "_Translations";

    /// <summary>
    ///     The name of the patch subfolder within the translations folder (default: "Patches").
    /// </summary>
    public const string PatchSubfolder = "Patches";

    /// <summary>
    ///     The name of the features subfolder within the translations folder (default: "Features").
    /// </summary>
    public const string FeaturesSubfolder = "Features";

    /// <summary>
    ///     The required fallback culture for translations (default: "en").
    /// </summary>
    public const string RequiredCulture = "en";
}