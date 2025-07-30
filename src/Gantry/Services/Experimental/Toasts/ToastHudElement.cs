using Cairo;
using Gantry.Extensions.DotNet;

namespace Gantry.Services.Experimental.Toasts;

/// <summary>
///     Represents a notification toast displayed on the HUD.
/// </summary>
internal class ToastHudElement : HudElement
{
    private readonly Toast _toast;
    private float _ttl;

    private int Position { get; set; }

    private static Queue<ToastHudElement> Active => G.Services.GetRequiredService<ToastService>().Active;

    /// <summary>
    ///     Initialises a new instance of the <see cref="ToastHudElement"/> class.
    /// </summary>
    /// <param name="capi">The core client API instance.</param>
    /// <param name="toast">The toast to display.</param>
    public ToastHudElement(ICoreClientAPI capi, Toast toast) : base(capi)
    {
        _toast = toast;
        _ttl = toast.ExpiryTime;
        ComposeDialogue();
    }

    /// <inheritdoc />
    public override bool ShouldReceiveMouseEvents() => false;

    /// <inheritdoc />
    public override void OnRenderGUI(float deltaTime)
    {
        base.OnRenderGUI(deltaTime);
        var dynamicText = SingleComposer.GetDynamicText("text");

        SingleComposer.Bounds.absOffsetY = Position * dynamicText.Bounds.absInnerHeight;
        dynamicText.Font = FadingFont();
        dynamicText.RecomposeText();

        if (Position == 0) _ttl -= deltaTime;
        if (_ttl >= 0) return;
        TryClose();
        Dispose();
    }

    /// <summary>
    ///     Terminates the toast before its expiry time.
    /// </summary>
    public void Kill() => _ttl = -.1f;

    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
        if (Active.Count > 0) Active.Dequeue();
        Active.InvokeForAll(p => p.Position--);
    }

    private void ComposeDialogue()
    {
        var text = _toast.Message;
        Position = Active.Count;

        var textBounds = ElementBounds.Fixed(EnumDialogArea.RightMiddle, 0, 0, text.Length * 12, 15);
        var dialogueBounds = ElementBounds.Fixed(EnumDialogArea.RightMiddle, 0, 350, textBounds.fixedWidth, textBounds.fixedHeight);

        SingleComposer = capi.Gui
            .CreateCompo("notification" + text + capi.Gui.OpenedGuis.Count + 1 + GetHashCode(), dialogueBounds)
            .AddDynamicText(text, FadingFont(), textBounds, "text")
            .Compose();

        var dynamicText = SingleComposer.GetDynamicText("text");
        SingleComposer.Bounds.absOffsetY = dynamicText.Bounds.absInnerHeight + Position * dynamicText.Bounds.absInnerHeight;

        TryOpen();
    }

    private CairoFont FadingFont()
    {
        var alpha = _ttl / 2.0;
        var colour = _toast.Colour.ToNormalisedRgba().With(p => p[2] = alpha);
        return CairoFont
            .WhiteDetailText()
            .WithColor(colour)
            .WithStroke([0.0, 0.0, 0.0, alpha], 1.0)
            .WithWeight(FontWeight.Bold)
            .WithOrientation(EnumTextOrientation.Right);
    }
}