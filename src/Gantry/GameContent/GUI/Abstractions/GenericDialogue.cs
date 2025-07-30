using Gantry.Core.Abstractions;
using Gantry.GameContent.Extensions.Gui;
using Gantry.GameContent.GUI.Helpers;
using Vintagestory.Client;

namespace Gantry.GameContent.GUI.Abstractions;

/// <summary>
///     Acts as a base class for basic, automatically sized dialogue boxes.
/// </summary>
/// <seealso cref="GuiDialog" />
public abstract class GenericDialogue : GuiDialog
{
    /// <summary>
    ///     Provides the core API surface for Gantry mods, exposing logging, dependency injection, localisation, mod metadata, and core services.
    ///     This interface is implemented by the Gantry core and injected into mod hosts and services, ensuring correct context and isolation per mod.
    /// </summary>
    public ICoreGantryAPI Gantry { get; }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="GenericDialogue"/> class.
    /// </summary>
    /// <param name="gantry">The core Gantry API.</param>
    protected GenericDialogue(ICoreGantryAPI gantry) : base(gantry.ApiEx.Client)
    {
        Gantry = gantry;
        ToggleKeyCombinationCode = GetType().Name;

        ClientSettings.Inst.AddWatcher<float>("guiScale", _ =>
        {
            PreCompose();
            Compose();
            RefreshValues();
        });
    }

    /// <summary>
    ///     The key combination string that toggles this GUI object.
    /// </summary>
    /// <value>The toggle key combination code.</value>
    public override string ToggleKeyCombinationCode { get; }

    /// <summary>
    ///     Attempts to open this dialogue.
    /// </summary>
    /// <returns>
    ///     Returns <see langword="true"/> if the dialogue window was opened correctly; otherwise, returns <see langword="false"/>
    /// </returns>
    public override bool TryOpen()
    {
        var openWindows = Gantry.ApiEx.Client.OpenedGuis;
        foreach (var gui in openWindows)
        {
            if (gui is not GuiDialog window) continue;
            if (window.ToggleKeyCombinationCode is null) continue;
            if (!window.ToggleKeyCombinationCode.Equals(ToggleKeyCombinationCode)) continue;
            window.Focus();
            return false;
        }
        var success = base.TryOpen();
        PreCompose();
        Compose();
        if (success) RefreshValues();
        return opened;
    }

    /// <summary>
    ///     Actions taken before the actual composition of the body of the dialogue.
    /// </summary>
    protected virtual void PreCompose()
    {
        // Base class does nothing.
    }

    /// <summary>
    ///     Composes the GUI components for this instance.
    /// </summary>
    protected virtual void Compose()
    {
        var composer = ComposeHeader().BeginChildElements(DialogueBounds);

        if (Modal) ComposeModalOverlay();

        ComposeBody(composer);

        composer
            .GetElements<GuiElementHoverText>()
            .ForEach(p => p.ZPosition = 50f);

        SingleComposer = composer
            .EndChildElements()
            .Compose();
    }

    /// <summary>
    ///     Sets the title of the dialogue box.
    /// </summary>
    /// <value>The raw, pre-localised, string literal to use for the title of the dialogue box.</value>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Sets the alignment of the form on the screen, when set to Fixed mode.
    /// </summary>
    /// <value>The <see cref="EnumDialogArea"/> alignment to set the window as.</value>
    protected EnumDialogArea Alignment { private get; set; } = EnumDialogArea.RightBottom;

    /// <summary>
    ///     The overall maximum bounds of the dialogue box.
    /// </summary>
    protected ElementBounds DialogueBounds { get; private set; } = ElementBounds.Empty;

    /// <summary>
    ///     Determines whether to allow the user to be able to move the form, within the bounds of the screen.
    /// </summary>
    protected bool Movable { get; set; }

    /// <summary>
    ///     Modal forms will remain the topmost form, and not allow throughput, until the form is closed.
    /// </summary>
    protected bool Modal { get; set; } = true;

    /// <summary>
    ///     Applies a transparency effect to the surrounding modal form.
    /// </summary>
    protected float ModalTransparency { get; set; }

    /// <summary>
    ///     Determines whether to display a title bar for this dialogue box.
    /// </summary>
    protected bool ShowTitleBar { get; set; } = true;

    /// <summary>
    ///     Determines whether to display an opaque background.
    /// </summary>
    protected bool TransparentBackground { get; set; }

    /// <summary>
    ///     Refreshes the displayed values on the form.
    /// </summary>
    protected virtual void RefreshValues()
    {
    }

    /// <summary>
    ///     Composes the header for the GUI.
    /// </summary>
    /// <param name="composer">The composer.</param>
    protected abstract void ComposeBody(GuiComposer composer);

    private void ComposeModalOverlay()
    {
        var fullScreenWidth = ScreenManager.Platform.WindowSize.Width;
        var fullScreenHeight = ScreenManager.Platform.WindowSize.Height;

        var composer = capi.Gui.CreateCompo(ToggleKeyCombinationCode,
                ElementBounds.Fixed(0, 0, fullScreenWidth, fullScreenHeight))
            .BeginChildElements(ElementBounds.Fixed(0, 0, fullScreenWidth, fullScreenHeight))
            .AddStaticCustomDraw(ElementBounds.Fixed(0, 0, fullScreenWidth, fullScreenHeight), (ctx, _, bounds) =>
            {
                ctx.Rectangle(0, 0, bounds.OuterWidth, bounds.OuterHeight);
                ctx.SetSourceRGB(0, 0, 0);
                ctx.PaintWithAlpha(ModalTransparency);
            })
            .EndChildElements();

        composer.zDepth -= 1;
        Composers["ModalOverlay"] = composer.Compose();
    }

    private GuiComposer ComposeHeader()
    {
        var dialogueBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(Alignment)
            .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, -GuiStyle.DialogToScreenPadding);

        DialogueBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        DialogueBounds.BothSizing = ElementSizing.FitToChildren;

        var composer = capi.Gui
            .CreateCompo(ToggleKeyCombinationCode, dialogueBounds);

        if (!TransparentBackground)
        {
            composer.AddShadedDialogBG(DialogueBounds);
        }

        if (!ShowTitleBar) return composer;

        return Movable ?
            composer.AddDialogTitleBar(Title, () => TryClose()) :
            composer.AddTitleBarWithNoMenu(Title, () => TryClose());
    }

    /// <summary>
    ///     Gets whether ability to grab the mouse cursor is disabled while
    ///     this dialog is opened. For example, the escape menu. (Default: false)
    /// </summary>
    public override bool DisableMouseGrab => true;
}