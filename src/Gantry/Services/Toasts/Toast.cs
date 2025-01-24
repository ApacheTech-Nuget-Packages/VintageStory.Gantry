using System.Drawing;
using ProtoBuf;

namespace Gantry.Services.Toasts;

/// <summary>
///     Represents a toast notification with a message, colour, and expiry time.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[ProtoContract]
public class Toast
{
    [ProtoMember(1)]
    private int _c;

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
    /// <param name="message">The message displayed by the toast notification.</param>
    /// <param name="colour">The colour of the toast notification.</param>
    /// <param name="expiryTime">The time, in seconds, after which the toast notification expires.</param>
    public Toast(string message, Color colour, float expiryTime = 2f)
    {
        Message = message;
        Colour = colour;
        ExpiryTime = expiryTime;
    }

    /// <summary>
    ///     The message displayed by the toast notification.
    /// </summary>
    [ProtoMember(2)]
    public string Message { get; set; } = "Hello, World!";

    /// <summary>
    ///     The colour of the toast notification.
    /// </summary>
    public Color Colour
    {
        get => Color.FromArgb(_c);
        set => _c = value.ToArgb();
    }

    /// <summary>
    ///     The time, in seconds, after which the toast notification expires. Defaults to 2 seconds.
    /// </summary>
    [ProtoMember(3)]
    public float ExpiryTime { get; set; } = 2f;

}