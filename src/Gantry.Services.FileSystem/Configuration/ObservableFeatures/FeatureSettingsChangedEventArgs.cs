using System;

namespace Gantry.Services.FileSystem.Configuration.ObservableFeatures
{
    /// <summary>
    ///     Contains data pertaining to an <see cref="ObservableFeature{T}"/>, as an observed property changes.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the strongly-typed POCO settings class.</typeparam>
    /// <seealso cref="System.EventArgs" />
    public class FeatureSettingsChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 	Initialises a new instance of the <see cref="FeatureSettingsChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="featureName">The name of the feature being observed.</param>
        /// <param name="featureObj">The strongly-typed feature settings object.</param>
        public FeatureSettingsChangedEventArgs(string featureName, T featureObj)
        {
            FeatureName = featureName;
            FeatureSettings = featureObj;
        }

        /// <summary>
        ///     Gets the name of the feature being observed.
        /// </summary>
        /// <value>The name of the feature.</value>
        public string FeatureName { get; }

        /// <summary>
        ///     Gets the strongly-typed feature settings object.
        /// </summary>
        /// <value>The feature settings.</value>
        public T FeatureSettings { get; }
    }
}