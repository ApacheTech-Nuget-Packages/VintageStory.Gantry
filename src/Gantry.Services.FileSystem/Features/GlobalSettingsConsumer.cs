using Gantry.Services.FileSystem.Configuration;

namespace Gantry.Services.FileSystem.Features
{
    /// <summary>
    ///     Represents a class that affects, or is affected by specific feature settings.
    /// </summary>
    /// <typeparam name="T">The settings file to use within the patches in this class.</typeparam>
    public abstract class GlobalSettingsConsumer<T> : SettingsConsumer<T> where T : FeatureSettings, new()
    {
        /// <summary>
        ///     Saves any changes to the mod settings file.
        /// </summary>
        protected override void SaveChanges()
        {
            ModSettings.Global.Save(Settings, FeatureName);
        }
    }
}