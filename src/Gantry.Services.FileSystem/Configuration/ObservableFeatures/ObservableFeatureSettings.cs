using System;
using System.ComponentModel;
using Gantry.Services.FileSystem.Enums;

namespace Gantry.Services.FileSystem.Configuration.ObservableFeatures
{
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
        public ObservableFeatureSettings(T instance, string featureName, FileScope scope) : base(instance)
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
        /// <returns>An instance of <see cref="ObservableFeatureSettings{T}"/>, which exposes the <c>PropertyChanged</c> event.</returns>
        public static ObservableFeatureSettings<T> Bind(T instance, string featureName, FileScope scope) => 
            new(instance, featureName, scope);

        private void OnPropertyChanged(object instance, string propertyName) => 
            ModSettings.For(_scope).Save(Object, _featureName);
    }
}
