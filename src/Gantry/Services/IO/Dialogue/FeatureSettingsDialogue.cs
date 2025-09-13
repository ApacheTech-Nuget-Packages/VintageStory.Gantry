using Gantry.Core.Abstractions;
using Gantry.GameContent.GUI.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Dialogue;

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
    /// <param name="gapi">The game's core API.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="featureName">Name of the feature.</param>
    protected FeatureSettingsDialogue(ICoreGantryAPI gapi, TFeatureSettings settings, string? featureName = null) :
        base(gapi)
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
        => Gantry.Lang.Translate($"{FeatureName}.Dialogue", path, args);

    /// <summary>
    ///     Saves the feature changes to the appropriate settings file based on the specified scope.
    /// </summary>
    /// <param name="scope">The scope to save the feature changes to (Global or World).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided scope is not recognised.</exception>
    protected void SaveFeatureChanges(ModFileScope scope = ModFileScope.World)
    {
        switch (scope)
        {
            case ModFileScope.Global:
                Gantry.Settings.Global.Save(Settings);
                break;
            case ModFileScope.World:
                Gantry.Settings.World.Save(Settings);
                break;
            case ModFileScope.Gantry:
                Gantry.Settings.Gantry.Save(Settings);
                break;
            default:
                throw new UnreachableException();
        }
    }
}