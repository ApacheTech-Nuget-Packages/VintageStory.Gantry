using System.Linq.Expressions;
using ProtoBuf;
using Gantry.Services.IO.Configuration.ObservableFeatures;

namespace Gantry.Services.IO.Configuration.Abstractions;

/// <summary>
///     Acts as a base class for all settings POCO Classes for a given feature.
/// </summary>
/// <seealso cref="IDisposable" />
public abstract class FeatureSettings<TSettings> : FeatureSettings where TSettings : FeatureSettings<TSettings>, new()
{
    [ProtoIgnore]
    internal Dictionary<string, List<PropertyChangedAction>> PropertyChangedDictionary { get; } = [];
    
    /// <summary>
    ///     Adds an action to be triggered when a specific property changes.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property that triggers the action.</typeparam>
    /// <param name="property">An expression representing the property to monitor for changes.</param>
    /// <param name="action">The action to execute when the property changes.</param>
    /// <exception cref="ArgumentException">Thrown when the provided expression does not represent a property.</exception>
    public string AddPropertyChangedAction<TProperty>(Expression<System.Func<TSettings, TProperty>> property, Action<TProperty> action)
    {
        if (property.Body is not MemberExpression memberExpression)
            throw new ArgumentException("The provided expression does not represent a property.");

        var propertyName = memberExpression.Member.Name;
        if (!PropertyChangedDictionary.TryGetValue(propertyName, out var value))
            PropertyChangedDictionary[propertyName] = value = [];

        var propertyChangedAction = new PropertyChangedAction(Guid.NewGuid().ToString(), p => action((TProperty)p));
        value.Add(propertyChangedAction);
        return propertyChangedAction.Id;
    }

    /// <summary>
    ///     Removes a previously added action that is triggered when a specific property changes.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property associated with the action.</typeparam>
    /// <param name="property">An expression representing the property for which the action should be removed.</param>
    /// <param name="actionName">The name of the action to remove.</param>
    /// <exception cref="ArgumentException">Thrown when the provided expression does not represent a property.</exception>
    public void RemovePropertyChangedAction<TProperty>(Expression<System.Func<TSettings, TProperty>> property, string actionName)
    {
        if (property.Body is not MemberExpression memberExpression)
            throw new ArgumentException("The provided expression does not represent a property.");

        var propertyName = memberExpression.Member.Name;
        if (!PropertyChangedDictionary.TryGetValue(propertyName, out var value)) return;
        value.RemoveAll(p => p.Id == actionName);
    }
}