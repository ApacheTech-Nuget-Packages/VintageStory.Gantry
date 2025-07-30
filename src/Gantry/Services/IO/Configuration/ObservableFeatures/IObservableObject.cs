namespace Gantry.Services.IO.Configuration.ObservableFeatures;

/// <summary>
///     Represents a class that notifies observers that a property value has changed within a wrapped POCO class.
/// </summary>
public interface IObservableObject
{
    /// <summary>
    ///     The instance of the object being observed.
    /// </summary>
    object Object { get; }

    /// <summary>
    ///     Sets a value indicating whether this <see cref="ObservableObject{T}"/> is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if active; otherwise, <c>false</c>.
    /// </value>
    bool Active { set; }

    /// <summary>
    ///     Occurs when a property value is changed, within the observed POCO class.
    /// </summary>
    Action<object, string>? OnObjectPropertyChanged { get; set; }

    /// <summary>
    ///     Removes the postfix patches from the observed item.
    /// </summary>
    void UnPatch();
}