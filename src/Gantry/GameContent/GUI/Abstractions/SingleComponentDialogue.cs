using Gantry.Core.Abstractions;
using Gantry.GameContent.GUI.Helpers;

namespace Gantry.GameContent.GUI.Abstractions;

/// <summary>
///     
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="GenericDialogue" />
public abstract class SingleComponentDialogue<T> : GenericDialogue where T : IGuiComposablePart, new()
{
    private readonly T _component;

    /// <summary>
    ///     Initialises a new instance of the <see cref="SingleComponentDialogue{T}"/> class.
    /// </summary>
    /// <param name="gapi">The client API.</param>
    protected SingleComponentDialogue(ICoreGantryAPI gapi) : base(gapi)
    {
        _component = new T { Bounds = DialogueBounds };
    }

    /// <summary>
    ///     Composes the header for the GUI.
    /// </summary>
    /// <param name="composer">The composer.</param>
    protected override void ComposeBody(GuiComposer composer)
    {
        composer.AddComposablePart(this, _component);
    }

    /// <summary>
    ///     Refreshes the displayed values on the form.
    /// </summary>
    protected override void RefreshValues()
    {
        _component.RefreshValues(SingleComposer);
    }
}