using System;
using System.Collections.Generic;
using System.ComponentModel;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration.ObservableFeatures;
using Gantry.Services.FileSystem.Enums;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gantry.Services.FileSystem.Configuration
{
    /// <summary>
    ///     Represents a settings file for the mod, in JSON format.
    /// </summary>
    /// <seealso cref="Abstractions.IJsonSettingsFile" />
    public class JsonSettingsFile : Abstractions.IJsonSettingsFile, IDisposable
    {
        private readonly FileScope _scope;
        private readonly Harmony _harmony;
        private Dictionary<string, IObservableObject> _observers;

        /// <summary>
        ///     Gets the underlying <see cref="IJsonModFile" /> that this instance wraps.
        /// </summary>
        /// <value>
        /// The file underlying JSON file from the file system.
        /// </value>
        public IJsonModFile File { get; }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="JsonSettingsFile"/> class.
        /// </summary>
        /// <param name="file">The underlying file, registered within the file system service.</param>
        /// <param name="scope">The scope that the settings file resides in.</param>
        /// <param name="harmony">The harmony instance to use to patch the files.</param>
        private JsonSettingsFile(IJsonModFile file, FileScope scope, Harmony harmony)
        {
            (File, _scope, _harmony) = (file, scope, harmony);
            _observers = new Dictionary<string, IObservableObject>();
        }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="JsonSettingsFile"/> class.
        /// </summary>
        /// <param name="file">The underlying file, registered within the file system service.</param>
        /// <param name="scope">The scope that the settings file resides in.</param>
        /// <param name="harmony">The harmony instance to use to patch the files.</param>
        public static JsonSettingsFile FromJsonFile(IJsonModFile file, FileScope scope, Harmony harmony) => new(file, scope, harmony);

        /// <summary>
        ///     Binds the specified feature to a POCO class object; dynamically adding an implementation of <see cref="INotifyPropertyChanged"/>, 
        ///     which saves changes to the underlying JSON file, whenever a property within the POCO is set.
        /// </summary>
        /// <remarks>
        ///     NOTE: Over-enthusiastic use of property setting within the POCO class, may result in excessive writes to the JSON file.
        ///           Sliders, in particular should be set to only fire on mouse up.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
        /// <param name="featureName">The name of the feature.</param>
        /// <returns>An object, that represents the settings for a given mod feature.</returns>
        public T Feature<T>(string featureName = null) where T: class, new()
        {
            featureName ??= typeof(T).Name.Replace("Settings", "");
            if (_observers.ContainsKey(featureName))
            {
                return (T)_observers[featureName].Object;
            }
            try
            {
                var json = File.ParseAs<JObject>();
                if (json is null) Save(json = JObject.FromObject(new T()), featureName);
                
                var obj = json.SelectToken($"$.Features.{featureName}");
                if (obj is null) Save(new T());
                var featureObj = obj?.ToObject<T>() ?? new T();
                
                var observer = ObservableFeatureSettings<T>.Bind(featureObj, featureName, _scope, _harmony);
                _observers.AddIfNotPresent(featureName, observer);

                observer.Active = true;
                return (T)observer.Object;
            }
            catch (Exception exception)
            {
                ApiEx.Current.Logger.Error($"Error loading feature `{featureName}` from file `{File.AsFileInfo().Name}`: {exception.Message}");
                ApiEx.Current.Logger.Error(exception.StackTrace);
                throw;
            }
        }

        /// <summary>
        ///     Saves the specified settings to file.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of object to parse the settings for the feature into.</typeparam>
        /// <param name="settings">The settings.</param>
        /// <param name="featureName">The name of the feature.</param>
        public void Save<T>(T settings, string featureName = null)
        {
            featureName ??= typeof(T).Name.Replace("Settings", "");
            var json = File.ParseAs<JObject>() ?? JObject.Parse("{ Features: {  } }");
            json.SelectToken("$.Features")[featureName] = JToken.FromObject(settings);
            File.SaveFrom(json.ToString(Formatting.Indented));
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var observer in _observers.Values)
            {
                observer.Active = false;
                observer.UnPatch();
            }
            _observers.Clear();
            _observers = null;
        }
    }
}