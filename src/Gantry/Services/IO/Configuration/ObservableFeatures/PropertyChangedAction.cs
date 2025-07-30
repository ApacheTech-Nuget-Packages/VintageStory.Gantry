namespace Gantry.Services.IO.Configuration.ObservableFeatures;

/// <summary>
///     Represents an action to be executed when a specific property changes.
/// </summary>
/// <param name="Id">The name of the property that triggers the action.</param>
/// <param name="Action">The action to be executed when the property changes.</param>
public record PropertyChangedAction(string Id, Action<object> Action);