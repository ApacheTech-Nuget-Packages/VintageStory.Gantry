using System;
using Gantry.Core.GameContent.GUI;
using Vintagestory.API.Client;
using Vintagestory.Client;
using Vintagestory.Client.NoObf;

namespace Gantry.Tests.AcceptanceMod.Features.Gui.Dialogue
{
    internal class TestDialogue : GenericDialogue
    {
        internal TestDialogue(ICoreClientAPI capi) : base(capi)
        {
            Title = "Acceptance Gui Test Window";
            Modal = true;
            ModalTransparency = 0.4f;
            Alignment = EnumDialogArea.CenterMiddle;
        }

        protected override void ComposeBody(GuiComposer composer)
        {
            var scaledWidth = Math.Min(800, ScreenManager.Platform.WindowSize.Width * 0.5) / ClientSettings.GUIScale;
            var scaledHeight = Math.Min(600, (ScreenManager.Platform.WindowSize.Height - 65) * 0.85) / ClientSettings.GUIScale;


            var outerBounds = ElementBounds
                .Fixed(EnumDialogArea.LeftTop, 0, 0, scaledWidth, 35);

            var insetBounds = outerBounds
                .BelowCopy(0, 3)
                .WithFixedSize(scaledWidth, scaledHeight);

            var controlRowBoundsLeftFixed = ElementBounds
                .FixedSize(100, 30)
                .WithFixedPadding(10, 2)
                .WithAlignment(EnumDialogArea.LeftFixed);

            var controlRowBoundsRightFixed = ElementBounds
                .FixedSize(100, 30)
                .WithFixedPadding(10, 2)
                .WithAlignment(EnumDialogArea.RightFixed);

            composer.AddInset(insetBounds);

            composer.AddButton("Open Message Box", OnOpenMessageBox, controlRowBoundsLeftFixed.FixedUnder(insetBounds, 10.0),
                CairoFont.ButtonText(), EnumButtonStyle.Normal, EnumTextOrientation.Center, "btnMessageBox");

            composer.AddButton("Close", TryClose, controlRowBoundsRightFixed.FixedUnder(insetBounds, 10.0),
                CairoFont.ButtonText(), EnumButtonStyle.Normal, EnumTextOrientation.Center, "btnClose");
        }

        private bool OnOpenMessageBox()
        {
            MessageBox.Show("Acceptance Tests", "We have message box support!", ButtonLayout.Ok, () => TryClose());
            return true;
        }
    }
}