namespace Gantry.Core.GameContent.GUI.Abstractions;

/// <summary>
///     A simple GUI that contains a single column of buttons, or other elements.
/// </summary>
public abstract class RowedButtonMenuDialogue : GenericDialogue
{
    private float _currentRow;

    /// <summary>
    /// 	Initialises a new instance of the <see cref="RowedButtonMenuDialogue"/> class.
    /// </summary>
    /// <param name="capi">The client API.</param>
    protected RowedButtonMenuDialogue(ICoreClientAPI capi) : base(capi)
    {
        Alignment = EnumDialogArea.CenterMiddle;
    }

    /// <summary>
    ///     Sets the language entry prefix.
    /// </summary>
    /// <value>The language entry prefix.</value>
    protected virtual string LangEntryPrefix { get; } = string.Empty;

    /// <summary>
    ///     Sets the width of the buttons.
    /// </summary>
    /// <value>The width of the buttons.</value>
    protected virtual float ButtonWidth { private get; init; } = 350f;

    /// <summary>
    ///     Sets the height offset.
    /// </summary>
    /// <value>The height offset.</value>
    protected virtual float HeightOffset { private get; init; }

    /// <summary>
    ///     Gets an entry from the language files, for the feature this instance is representing.
    /// </summary>
    /// <param name="code">The entry to return.</param>
    /// <returns>A localised <see cref="string"/>, for the specified language file code.</returns>
    protected string LangEntry(string code) => LangEx.FeatureString(LangEntryPrefix, code);

    /// <summary>
    ///     Adds a button to the dialogue, and increments the current row.
    /// </summary>
    /// <param name="composer">The composer.</param>
    /// <param name="langEntry">The title.</param>
    /// <param name="onClick">The on click handler.</param>
    protected void AddButton(GuiComposer composer, string langEntry, ActionConsumable onClick)
    {
        composer.AddSmallButton(langEntry, onClick, ButtonBounds(ButtonWidth, HeightOffset));
    }

    /// <summary>
    ///     Increments the current row.
    /// </summary>
    protected void IncrementRow() => _currentRow += 0.5f;

    private ElementBounds ButtonBounds(double width, double height)
    {
        IncrementRow();
        return ElementStdBounds
            .MenuButton(_currentRow, EnumDialogArea.LeftFixed)
            .WithFixedOffset(0, height)
            .WithFixedSize(width, 30);
    }
}