using System;
using System.Collections.Generic;
using System.ComponentModel;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration.ObservableFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gantry.Services.FileSystem.Configuration
{
    /// <summary>
    ///     Represents a settings file for the mod, in JSON format.
    /// </summary>
    /// <seealso cref="Abstractions.IJsonSettingsFile" />
    public class JsonSettingsFile : Abstractions.IJsonSettingsFile
    {
        private readonly List<IDisposable> _observers = new();

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
        private JsonSettingsFile(IJsonModFile file) => File = file;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="JsonSettingsFile"/> class.
        /// </summary>
        /// <param name="file">The underlying file, registered within the file system service.</param>
        public static JsonSettingsFile FromJsonFile(IJsonModFile file) => new(file);

        /// <summary>
        ///     Binds the specified feature to a POCO class object; dynamically adding an implementation of <see cref="INotifyPropertyChanged"/>, 
        ///     which saves changes to the underlying JSON file, whenever a property within the POCO is set.
        /// </summary>
        /// <remarks>
        ///     NOTE: Over-enthusiastic use of property setting within the POCO class, may result in excessive writes to the JSON file.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of object to parse the settings for the feature into.</typeparam>
        /// <param name="featureName">The name of the feature.</param>
        /// <returns>An object, that represents the settings for a given mod feature.</returns>
        public T Feature<T>(string featureName = null) where T: class, new()
        {
            featureName ??= typeof(T).Name.Replace("Settings", "");
            try
            {
                var json = File.ParseAs<JObject>();
                if (json is null)
                {
                    var defaultData = new T();
                    Save(json = JObject.FromObject(defaultData), featureName);
                }
                T featureObj;
                var obj = json.SelectToken($"$.Features.{featureName}");
                if (obj is null)
                {
                    featureObj = new T();
                    var args = new FeatureSettingsChangedEventArgs<T>(featureName, featureObj);
                    OnPropertyChanged(args);
                }
                else
                {
                    featureObj = obj.ToObject<T>();
                }

                var observer = ObservableFeature<T>.Bind(featureName, featureObj);
                _observers.AddIfNotPresent(observer);
                observer.PropertyChanged += OnPropertyChanged;
                return featureObj;
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
            var json = File.ParseAs<JObject>() ?? JObject.Parse("{ \"Features\": {  } }");
            var featureObj = json.SelectToken($"$.Features.{featureName}");
            switch (featureObj)
            {
                case null:
                    json.SelectToken("$.Features")[featureName] = JToken.FromObject(settings);
                    break;
                default:
                    featureObj.Replace(JToken.FromObject(settings));
                    break;
            }
            File.SaveFrom(json.ToString(Formatting.Indented));
        }

        private void OnPropertyChanged<T>(FeatureSettingsChangedEventArgs<T> args) 
            => Save(args.FeatureSettings, args.FeatureName);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _observers.Purge();
        }
    }
}