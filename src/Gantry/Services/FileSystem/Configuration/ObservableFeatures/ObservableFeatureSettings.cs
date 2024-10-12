using System.ComponentModel;
using Gantry.Core;
using Gantry.Services.FileSystem.Enums;
using HarmonyLib;

namespace Gantry.Services.FileSystem.Configuration.ObservableFeatures;

/// <summary>
///     Notifies observers that a property value has changed within a wrapped POCO class.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of object to watch.</typeparam>
/// <seealso cref="IDisposable" />
public class ObservableFeatureSettings<T> : ObservableObject<T> where T : class, new()
{
    private readonly string _featureName;
    private readonly FileScope _scope;

    /// <summary>
    ///     Initialises a new instance of the <see cref="ObservableFeatureSettings{T}"/> class.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="featureName">Name of the feature.</param>
    /// <param name="scope">Scope of the feature.</param>
    /// <param name="harmony">The harmony instance to use to patch the files.</param>
    public ObservableFeatureSettings(T instance, string featureName, FileScope scope, Harmony harmony) : base(instance, harmony)
    {
        _featureName = featureName;
        _scope = scope;
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
    /// <returns>An instance of <see cref="ObservableFeatureSettings{T}"/>, which exposes the <c>PropertyChanged</c> event.</returns>
    public static ObservableFeatureSettings<T> Bind(T instance, string featureName, FileScope scope, Harmony harmony) => 
        new(instance, featureName, scope, harmony);

    private void OnPropertyChanged(object instance, string propertyName)
    {
        try
        {
            if (!Active) return;
            if (Object is null) return;
            if (string.IsNullOrEmpty(_featureName)) return;
            var settings = ModSettings.For(_scope);
            if (settings is null) return;
            settings.Save(Object, _featureName);
        }
        catch (Exception ex)
        {
            ApiEx.Current.Logger.Error(ex);
        }
    }
}