using System;

namespace Gantry.Services.FileSystem.Configuration.ObservableFeatures
{
    /// <summary>
    ///     Represents a method that will handle an event being raised, when feature settings are changed within a settings file.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of feature object being observed.</typeparam>
    /// <param name="args">The arguments being sent to the handler.</param>
    public delegate void FeatureSettingsChangedEventHandler<T>(FeatureSettingsChangedEventArgs<T> args);
}