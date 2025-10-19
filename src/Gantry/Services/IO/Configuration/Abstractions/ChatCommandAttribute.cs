namespace Gantry.Services.IO.Configuration.Abstractions;

/// <summary>
///     Indicates that a property is associated with a chat command.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ChatCommandAttribute(string name) : Attribute
{
    /// <summary>
    ///     The name of the chat command.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     The default value for the property.
    /// </summary>
    public object? DefaultValue { get; init; }

    /// <summary>
    ///     The maximum value allowed for the property.
    /// </summary>
    public object? MaxValue { get; set; }

    /// <summary>
    ///     The minimum value allowed for the property.
    /// </summary>
    public object? MinValue { get; set; }

    /// <summary>
    ///     The aliases for the chat command.
    /// </summary>
    public string[]? Aliases { get; set; }

    /// <summary>
    ///     The list of words associated with the chat command.
    /// </summary>
    public string[]? WordList { get; set; }
}