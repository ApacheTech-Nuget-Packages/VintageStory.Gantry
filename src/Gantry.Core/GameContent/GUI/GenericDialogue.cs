using Gantry.Core.Extensions.GameContent.Gui;
using Gantry.Core.GameContent.GUI.Helpers;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.Client;

namespace Gantry.Core.GameContent.GUI
{
    /// <summary>
    ///     Acts as a base class for basic, automatically sized dialogue boxes.
    /// </summary>
    /// <seealso cref="GuiDialog" />
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public abstract class GenericDialogue : GuiDialog
    {
        /// <summary>
        /// 	Initialises a new instance of the <see cref="GenericDialogue"/> class.
        /// </summary>
        /// <param name="capi">The client API.</param>
        protected GenericDialogue(ICoreClientAPI capi) : base(capi)
        {
            ToggleKeyCombinationCode = GetType().Name;
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
            var openWindows = ApiEx.Client.OpenedGuis;
            foreach (var gui in openWindows)
            {
                if (gui is not GuiDialog window) continue;
                if (window.ToggleKeyCombinationCode is null) continue;
                if (!window.ToggleKeyCombinationCode.Equals(ToggleKeyCombinationCode)) continue;
                window.Focus();
                return false;
            }
            var success = base.TryOpen();
            Compose();
            if (success) RefreshValues();
            return opened;
        }

        /// <summary>
        ///     Composes the GUI components for this instance.
        /// </summary>
        protected virtual void Compose()
        {
            var composer = ComposeHeader().BeginChildElements(DialogueBounds);

            if (Modal)
            {
                ComposeModalOverlay();
            }

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
        public string Title { private get; set; }

        /// <summary>
        ///     Sets the alignment of the form on the screen, when set to Fixed mode.
        /// </summary>
        /// <value>The <see cref="EnumDialogArea"/> alignment to set the window as.</value>
        protected EnumDialogArea Alignment { private get; set; } = EnumDialogArea.RightBottom;

        /// <summary>
        ///     The overall maximum bounds of the dialogue box.
        /// </summary>
        protected ElementBounds DialogueBounds { get; private set; }

        /// <summary>
        ///     Determines whether or not to allow the user to be able to move the form, within the bounds of the screen.
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
        ///     Determines whether or not to display a title bar for this dialogue box.
        /// </summary>
        protected bool ShowTitleBar { get; set; } = true;

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
                .CreateCompo(ToggleKeyCombinationCode, dialogueBounds)
                .AddShadedDialogBG(DialogueBounds);

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
}