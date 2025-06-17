using Gantry.Core.GameContent.GUI.Abstractions;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem.Dialogue;

/// <summary>
///     Acts as a base class for dialogue boxes that 
/// </summary>
/// <typeparam name="TFeatureSettings">The strongly-typed settings for the feature under use.</typeparam>
/// <seealso cref="GenericDialogue" />
public abstract class FeatureSettingsDialogue<TFeatureSettings> : GenericDialogue
    where TFeatureSettings : class, new()
{
    /// <summary>
    ///     The strongly-typed settings for the feature under use.
    /// </summary>
    /// <value>The strongly-typed settings for the feature under use.</value>
    protected TFeatureSettings Settings { get; }

    /// <summary>
    ///     Gets the name of the feature.
    /// </summary>
    /// <value>The name of the feature.</value>
    protected string FeatureName { get; }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="FeatureSettingsDialogue{TFeatureSettings}"/> class.
    /// </summary>
    /// <param name="capi">The capi.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="featureName">Name of the feature.</param>
    protected FeatureSettingsDialogue(ICoreClientAPI capi, TFeatureSettings settings, string? featureName = null) :
        base(capi)
    {
        Settings = settings;
        FeatureName = featureName ?? typeof(TFeatureSettings).Name.Replace("Settings", "");
        Title = T("Title");
    }

    /// <summary>
    ///     Returns a localised string for the specified path and arguments, using the feature's dialogue language file.
    /// </summary>
    /// <param name="path">The localisation path within the feature's dialogue file.</param>
    /// <param name="args">Optional arguments to format the localised string.</param>
    /// <returns>A localised <see cref="string"/> for the specified language file code.</returns>
    protected string T(string path, params object[] args)
        => LangEx.FeatureString($"{FeatureName}.Dialogue", path, args);

    /// <summary>
    ///     Saves the feature changes to the appropriate settings file based on the specified scope.
    /// </summary>
    /// <param name="scope">The scope to save the feature changes to (Global or World).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided scope is not recognised.</exception>
    protected void SaveFeatureChanges(FileScope scope = FileScope.World)
    {
        switch (scope)
        {
            case FileScope.Global:
                ModSettings.Global.Save(Settings);
                break;
            case FileScope.World:
                ModSettings.World.Save(Settings);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
        }
    }
}