using System.ComponentModel;
using Gantry.Core.Abstractions;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;

namespace Gantry.Services.IO.Configuration.ObservableFeatures;

/// <summary>
///     Notifies observers that a property value has changed within a wrapped POCO class.
/// </summary>
/// <typeparam name="TSettings">The <see cref="Type"/> of object to watch.</typeparam>
/// <seealso cref="IDisposable" />
public class ObservableFeatureSettings<TSettings> : ObservableObject<TSettings> where TSettings : FeatureSettings<TSettings>, new()
{
    private readonly string _featureName;
    private readonly ModFileScope _scope;
    private readonly ICoreGantryAPI _gantry;

    /// <summary>
    ///     Initialises a new instance of the <see cref="ObservableFeatureSettings{T}"/> class.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="featureName">Name of the feature.</param>
    /// <param name="scope">Scope of the feature.</param>
    /// <param name="harmony">The harmony instance to use to patch the files.</param>
    /// <param name="gantry">The gantry API instance to use for saving settings.</param>
    public ObservableFeatureSettings(TSettings instance, string featureName, ModFileScope scope, Harmony harmony, ICoreGantryAPI gantry) 
        : base(instance, harmony, gantry)
    {
        _featureName = featureName;
        _scope = scope;
        _gantry = gantry;
        OnObjectPropertyChanged = OnPropertyChanged;
    }

    /// <summary>
    ///     Binds the specified feature to a POCO class object; dynamically adding an implementation of <see cref="INotifyPropertyChanged"/>, 
    ///     raising an event every time a property within the POCO class, is set.
    /// </summary>
    /// <param name="featureName">The name of the feature being observed.</param>
    /// <param name="instance">The instance of the POCO class that manages the feature settings.</param>
    /// <param name="scope">Scope of the feature.</param>
    /// <param name="harmony">The harmony instance to use to patch the files.</param>
    /// <param name="gantry">The gantry API instance to use for saving settings.</param>
    /// <returns>An instance of <see cref="ObservableFeatureSettings{T}"/>, which exposes the <c>PropertyChanged</c> event.</returns>
    public static ObservableFeatureSettings<TSettings> Bind(TSettings instance, string featureName, ModFileScope scope, Harmony harmony, ICoreGantryAPI gantry) => 
        new(instance, featureName, scope, harmony, gantry);

    private void OnPropertyChanged(object instance, string propertyName)
    {
        try
        {            
            if (!Active) return;
            if (Object is null) return;
            if (string.IsNullOrEmpty(_featureName)) return;
            var settings = _gantry.Settings.For(_scope);
            if (settings is null) return;
            if (!_gantry.Side.Is(settings.Side)) return;
            settings.Save(Object, _featureName);

            if (!((TSettings)instance).PropertyChangedDictionary.TryGetValue(propertyName, out var actions)) return;
            var value = instance.GetType().GetProperty(propertyName)?.GetValue(instance);
            if (value is null) return;
            actions.ForEach(action => action.Action(value));
        }
        catch (Exception ex)
        {
            _gantry.Logger.Error($"Failed to save settings for feature '{_featureName}' in scope '{_scope}': {ex.Message}");
            _gantry.Logger.Error(ex);
        }
    }
}