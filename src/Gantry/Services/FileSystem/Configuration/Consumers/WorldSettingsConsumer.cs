using Gantry.Services.FileSystem.Configuration.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.Configuration.Consumers
{
    /// <summary>
    ///     Represents a class that affects, or is affected by specific feature settings.
    /// </summary>
    /// <typeparam name="T">The settings file to use within the patches in this class.</typeparam>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class WorldSettingsConsumer<T> : ISettingsConsumer where T : FeatureSettings, new()
    {
        internal static void Initialise()
        {
            Settings = null;
            Settings = ModSettings.World?.Feature<T>();
        }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>
        ///     The settings.
        /// </value>
        protected static T Settings { get; private set; }

        /// <summary>
        ///     Gets or sets the name of the feature.
        /// </summary>
        /// <value>
        ///     The name of the feature.
        /// </value>
        protected static string FeatureName => typeof(T).Name.Replace("Settings", "");

        /// <summary>
        ///     Saves any changes to the mod settings file.
        /// </summary>
        protected void SaveChanges()
        {
            ModSettings.World.Save(Settings, FeatureName);
        }
    }
}