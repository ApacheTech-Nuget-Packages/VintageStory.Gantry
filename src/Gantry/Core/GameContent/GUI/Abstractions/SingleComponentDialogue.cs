using Gantry.Core.GameContent.GUI.Helpers;

namespace Gantry.Core.GameContent.GUI.Abstractions;

/// <summary>
///     
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="GenericDialogue" />
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public abstract class SingleComponentDialogue<T> : GenericDialogue where T : IGuiComposablePart, new()
{
    private readonly T _component;

    /// <summary>
    ///     Initialises a new instance of the <see cref="SingleComponentDialogue{T}"/> class.
    /// </summary>
    /// <param name="capi">The client API.</param>
    protected SingleComponentDialogue(ICoreClientAPI capi) : base(capi)
    {
        _component = new T { Bounds = DialogueBounds };
    }

    /// <summary>
    ///     Composes the header for the GUI.
    /// </summary>
    /// <param name="composer">The composer.</param>
    protected override void ComposeBody(GuiComposer composer)
    {
        composer.AddComposablePart(_component);
    }

    /// <summary>
    ///     Refreshes the displayed values on the form.
    /// </summary>
    protected override void RefreshValues()
    {
        _component.RefreshValues(SingleComposer);
    }
}