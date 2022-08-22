using Gantry.Services.FileSystem.Configuration;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.Features
{
    /// <summary>
    ///     Represents a class that affects, or is affected by specific feature settings.
    /// </summary>
    /// <typeparam name="T">The settings file to use within the patches in this class.</typeparam>
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public abstract class SettingsConsumer<T> where T : FeatureSettings, new()
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="SettingsConsumer{T}"/> class.
        /// </summary>
        internal SettingsConsumer() { }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>
        ///     The settings.
        /// </value>
        public static T Settings { get; } = ModSettings.World.Feature<T>();

        /// <summary>
        ///     Gets or sets the name of the feature.
        /// </summary>
        /// <value>
        ///     The name of the feature.
        /// </value>
        public static string FeatureName { get; } = typeof(T).Name.Replace("Settings", "");

        /// <summary>
        ///     Saves any changes to the mod settings file.
        /// </summary>
        protected abstract void SaveChanges();
    }
}