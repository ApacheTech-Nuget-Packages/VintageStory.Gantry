using System;
using System.ComponentModel;
using Gantry.Services.FileSystem.Configuration.DynamicNotifyPropertyChanged;

namespace Gantry.Services.FileSystem.Configuration.ObservableFeatures
{
    /// <summary>
    ///     Notifies observers that a property value has changed within a wrapped POCO class.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to watch.</typeparam>
    /// <seealso cref="IDisposable" />
    public class ObservableFeature<T> : IDisposable where T : class
    {
        private readonly DynamicNotifyPropertyChanged<T> _observedInstance;
        private static ObservableFeature<T> _instance;
        private readonly string _featureName;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="ObservableFeature{T}" /> class.
        /// </summary>
        /// <param name="featureName">The name of the feature to be observed</param>
        /// <param name="instance">The instance to watch.</param>
        private ObservableFeature(string featureName, T instance)
        {
            _featureName = featureName;
            _observedInstance = DynamicNotifyPropertyChanged<T>.Bind(instance);
            _observedInstance.PropertyChanged += OnObservedInstancePropertyChanged;
        }

        /// <summary>
        ///     Binds the specified feature to a POCO class object; dynamically adding an implementation of <see cref="INotifyPropertyChanged"/>, 
        ///     raising an event every time a property within the POCO class, is set.
        /// </summary>
        /// <param name="featureName">The name of the feature being observed.</param>
        /// <param name="instance">The instance of the POCO class that manages the feature settings.</param>
        /// <returns>An instance of <see cref="ObservableFeature{T}"/>, which exposes the <c>PropertyChanged</c> event.</returns>
        public static ObservableFeature<T> Bind(string featureName, T instance)
        {
            return _instance ??= new ObservableFeature<T>(featureName, instance);
        }

        /// <summary>
        ///     Occurs when a property value is changed, within the observed POCO class.
        /// </summary>
        public event FeatureSettingsChangedEventHandler<T> PropertyChanged;

        private void OnObservedInstancePropertyChanged(DynamicPropertyChangedEventArgs<T> args)
        {
            var featureArgs = new FeatureSettingsChangedEventArgs<T>(_featureName, args.Instance);
            PropertyChanged?.Invoke(featureArgs);
        }

        /// <summary>
        ///     Un-patches the dynamic property watch on the POCO class. 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _observedInstance.Dispose();
            _instance = null;
        }
    }
}
