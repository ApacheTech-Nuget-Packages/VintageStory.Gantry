using Gantry.Features.GantryChatCommands.ClientCommands;
using Gantry.Services.BrighterChat;

namespace Gantry.Features.GantryChatCommands.Systems;

internal class GantryChatClientSystem : ClientSubsystem
{
    public override void StartClientSide(ICoreClientAPI api)
    {
        var clientCommand = api.ChatCommands.GetOrCreate("gantry")
            .WithDescription("Actions to control the Gantry MDK, and associated mods.");

        var subCommands = clientCommand.AllSubcommands;

        if (!subCommands.ContainsKey("world"))
        {
            clientCommand
                .BeginSubCommand("world")
                .WithDescription("Show the world ID.")
                .HandleWith(_ => TextCommandResult.Success(api.World.SavegameIdentifier))
                .EndSubCommand();
        }

        //if (!subCommands.ContainsKey("debug"))
        //{
        //    clientCommand
        //        .BeginSubCommand("debug")
        //        .WithDescription("Toggle debug mode for Gantry enabled mods. This will add a lot of information to the log files, and may decrease the performance of the game while active.")
        //        .WithArgs(api.ChatCommands.Parsers.OptionalWord("modid"), api.ChatCommands.Parsers.Bool("debug-mode"))
        //        .WithMediatedHandler<CmdClientDebug>()
        //        .EndSubCommand();
        //}

        //if (!subCommands.ContainsKey("reset"))
        //{
        //    clientCommand
        //        .BeginSubCommand("reset")
        //        .WithDescription("Confirm with your player ID to reset the settings of a specific Gantry mod, given its mod id")
        //        .WithArgs(api.ChatCommands.Parsers.Word("playerId"), api.ChatCommands.Parsers.Word("modid"))
        //        .WithMediatedHandler<CmdClientResetMod>()
        //        .EndSubCommand();
        //}

        //if (!subCommands.ContainsKey("factoryreset"))
        //{
        //    clientCommand
        //        .BeginSubCommand("factoryreset")
        //        .WithDescription("Confirm with your player ID to perform a full factory reset on all Gantry mods that have been used with the game. This is a global wipe of ALL settings for ALL mods. Take care when using this command.")
        //        .WithArgs(api.ChatCommands.Parsers.Word("playerId"))
        //        .WithMediatedHandler<CmdClientFactoryReset>()
        //        .EndSubCommand();
        //}

        //if (!subCommands.ContainsKey("client-crash"))
        //{
        //    clientCommand
        //        .BeginSubCommand("client-crash")
        //        .WithDescription("Triggers a deliberate client-side crash.")
        //        .WithMediatedHandler<CmdClientCrash>()
        //        .EndSubCommand();   
        //}
    }
}