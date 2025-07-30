using System.ComponentModel;
using System.Drawing;
using ProtoBuf;

namespace Gantry.Services.Experimental.Toasts;

/// <summary>
///     Represents a toast notification with a message, colour, and expiry time.
/// </summary>
[ProtoContract]
internal class Toast
{
    [ProtoMember(1)]
    private int _colour;

    /// <summary>
    ///     Initialises a new instance of the <see cref="Toast"/> class.
    /// </summary>
    [JsonConstructor]
    public Toast()
    {
        // INTENTIONALLY BLANK
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="Toast"/> class.
    /// </summary>
    /// <param name="colour">The colour of the toast notification.</param>
    /// <param name="expiryTime">The time, in seconds, after which the toast notification expires.</param>
    public Toast(Color colour, float expiryTime = 2f)
    {
        Colour = colour;
        ExpiryTime = expiryTime;
    }

    /// <summary>
    ///     The message displayed by the toast notification.
    /// </summary>
    [ProtoMember(2)]
    public required string Message { get; set; }

    /// <summary>
    ///     The colour of the toast notification.
    /// </summary>
    public Color Colour
    {
        get => Color.FromArgb(_colour);
        set => _colour = value.ToArgb();
    }

    /// <summary>
    ///     The time, in seconds, after which the toast notification expires. Defaults to 2 seconds.
    /// </summary>
    [ProtoMember(3)]
    [DefaultValue(2f)]
    public float ExpiryTime { get; set; } = 2f;

}