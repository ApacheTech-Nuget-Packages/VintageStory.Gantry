using ApacheTech.Common.FunctionalCSharp.Monads.Extensions;
using Gantry.Core.Extensions.Functional;
using Vintagestory.API.Datastructures;

namespace Gantry.Core.Subsystems;

/// <summary>
///     Provides an event bus for subscribing, publishing, and handling events across Gantry mods.
/// </summary>
public class GantryEventBus : UniversalSubsystem
{
    private const string EventDomain = nameof(GantryEventBus);
    private static readonly Dictionary<string, (Type Type, Delegate Action)> Listeners = [];

    /// <inheritdoc />
    public override double ExecuteOrder() => double.NegativeInfinity;

    /// <inheritdoc />
    public override void StartPreUniversal(ICoreAPI api)
    {
        UApi.Event.RegisterEventBusListener(OnEvent, double.PositiveInfinity);
        base.StartPreUniversal(api);
    }

    /// <summary>
    ///     Subscribes a listener to an event of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="action">The action to invoke when the event is triggered.</param>
    public static void Subscribe<T>(Action<T> action)
        => Listeners[EventName<T>()] = (typeof(T), action);

    /// <summary>
    ///     Unsubscribes the listener for an event of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    public static void Unsubscribe<T>()
        => Listeners.RemoveIfPresent(EventName<T>());

    /// <summary>
    ///     Publishes an event of the specified type with the provided message.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="message">The event data to publish.</param>
    public static void Publish<T>(T message) 
        => message.ToIdentity()
            .Bind(x => JsonConvert.SerializeObject(x))
            .Bind(x => new StringAttribute(x))
            .Invoke(x => UApi.Event.PushEvent(EventName<T>(), x));

    /// <summary>
    ///     Gets the name of the event for the specified type.
    /// </summary>
    /// <typeparam name="T"> The type of the event.</typeparam>
    /// <returns>The fully qualified name of the event.</returns>
    public static string EventName<T>() 
        => $"{EventDomain}.{typeof(T).Name}";

    /// <summary>
    ///     Handles incoming events from the event bus, invoking subscribed listeners.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="handling">Specifies how the event should be handled.</param>
    /// <param name="data">The event data.</param>
    private void OnEvent(string eventName, ref EnumHandling handling, IAttribute data)
    {
        if (!eventName.StartsWith(EventDomain)) return;
        if (!Listeners.TryGetValue(eventName, out var listener)) return;

        var (type, action) = listener;

        var json = (string)data.GetValue();
        var deserialisedObject = JsonConvert.DeserializeObject(json, type);

        action.DynamicInvoke(deserialisedObject);
    }
}
