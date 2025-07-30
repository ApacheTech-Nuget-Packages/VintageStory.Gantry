using Gantry.Core.Abstractions;
using Gantry.Services.IO.Abstractions.Contracts;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.Configuration.ObservableFeatures;
using Gantry.Services.IO.DataStructures;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Gantry.Services.IO.Configuration;

/// <summary>
///     Represents a settings file for the mod, in JSON format.
/// </summary>
public class JsonSettingsFile : IJsonSettingsFile, IDisposable
{
    private readonly ModFileScope _scope;
    private readonly ICoreGantryAPI _gantry;
    private readonly Harmony _harmony;
    private readonly Dictionary<string, IObservableObject> _observers = [];

    /// <summary>
    ///     Gets the underlying <see cref="IJsonModFile" /> that this instance wraps.
    /// </summary>
    /// <value>
    ///     The file underlying JSON file from the file system.
    /// </value>
    public IJsonModFile File { get; }

    /// <summary>
    ///     Whether the settings are for the client, or the server.
    /// </summary>
    /// <value>
    ///     The App Side the settings file runs on.
    /// </value>
    public EnumAppSide Side { get; }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonSettingsFile"/> class.
    /// </summary>
    /// <param name="file">The underlying file, registered within the file system service.</param>
    /// <param name="gantry">The gantry API instance to use for saving settings.</param>
    /// <param name="scope">The scope that the settings file resides in.</param>
    /// <param name="harmony">The harmony instance to use to patch the files.</param>
    private JsonSettingsFile(IJsonModFile file, ICoreGantryAPI gantry, ModFileScope scope, Harmony harmony)
    {
        (File, Side, _gantry, _scope, _harmony) = (file, gantry.Side, gantry, scope, harmony);
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="JsonSettingsFile"/> class.
    /// </summary>
    /// <param name="file">The underlying file, registered within the file system service.</param>
    /// <param name="gantry">The gantry API instance to use for saving settings.</param>
    /// <param name="scope">The scope that the settings file resides in.</param>
    /// <param name="harmony">The harmony instance to use to patch the files.</param>
    public static JsonSettingsFile FromJsonFile(IJsonModFile file, ICoreGantryAPI gantry, ModFileScope scope, Harmony harmony) 
        => new(file, gantry, scope, harmony);

    /// <summary>
    ///     Binds the specified feature to a POCO class object; dynamically adding an implementation of <see cref="INotifyPropertyChanged"/>, 
    ///     which saves changes to the underlying JSON file, whenever a property within the POCO is set.
    /// </summary>
    /// <remarks>
    ///     NOTE: Over-enthusiastic use of property setting within the POCO class, may result in excessive writes to the JSON file.
    ///           Sliders, in particular should be set to only fire on mouse up.
    /// </remarks>
    /// <typeparam name="TSettings">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
    /// <param name="featureName">The name of the feature.</param>
    /// <returns>An object, that represents the settings for a given mod feature.</returns>
    public TSettings Feature<TSettings>(string? featureName = null) where TSettings : FeatureSettings<TSettings>, new()
    {
        featureName ??= typeof(TSettings).Name.Replace("Settings", "");
        if (_observers.TryGetValue(featureName, out var cachedObserver))
        {
            return (TSettings)cachedObserver.Object;
        }
        try
        {
            var json = File.ParseAs<JObject>();
            if (json is null) Save(json = JObject.FromObject(new TSettings()), featureName);
                
            var obj = json.SelectToken($"$.Features.{featureName}");
            if (obj is null) Save(new TSettings());
            var featureObj = obj?.ToObject<TSettings>() ?? new TSettings();
                
            var observer = ObservableFeatureSettings<TSettings>.Bind(featureObj, featureName, _scope, _harmony, _gantry);
            _observers.AddIfNotPresent(featureName, observer);

            observer.Active = true;
            return (TSettings)observer.Object;
        }
        catch (Exception exception)
        {
            _gantry.Logger.Error($"Error loading feature `{featureName}` from file `{File.AsFileInfo().Name}`: {exception.Message}");
            _gantry.Logger.Error(exception.StackTrace);
            throw;
        }
    }

    /// <summary>
    ///     Saves the specified settings to file.
    /// </summary>
    /// <typeparam name="TSettings">The <see cref="Type" /> of object to parse the settings for the feature into.</typeparam>
    /// <param name="settings">The settings.</param>
    /// <param name="featureName">The name of the feature.</param>
    public void Save<TSettings>(TSettings? settings, string? featureName = null)
    {
        if (settings is null) return;
        featureName ??= typeof(TSettings).Name.Replace("Settings", "");

        // Parse the JSON file or initialise a new JObject if the file is empty
        var json = File.ParseAs<JObject>() ?? [];

        // Ensure the "Features" token exists and is an object.
        if (json["Features"] is not JObject features)
        {
            features = [];
            json["Features"] = features;
        }

        // Add or update the settings under the specified feature name.
        features[featureName] = JToken.FromObject(settings);

        // Save the updated JSON to the file.
        File.SaveFrom(json.ToString(Formatting.Indented));
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var observer in _observers.Values)
        {
            observer.Active = false;
            observer.UnPatch();
        }
        _observers.Clear();
    }
}