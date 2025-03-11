using Gantry.Core.GameContent.ChatCommands;
using Vintagestory.API.Util;

namespace Gantry.Core.Subsystems;

internal class GantryDiagnostics : UniversalSubsystem
{
    public override void StartClientSide(ICoreClientAPI api)
    {
        GantryCommand.Command.TryAddSubCommand("logs", p => p
            .WithDescription("Open the Gantry logs folder.")
            .HandleWith(_ => NetUtil.OpenUrlInBrowser(ModEx.LogDirectory.FullName)));
    }
}