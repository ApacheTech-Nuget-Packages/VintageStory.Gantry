﻿using Vintagestory.API.Server;

namespace Gantry.Features.GantryChatCommands.Systems;

internal class GantryChatServerSystem : ServerSubsystem
{
    public override void StartServerSide(ICoreServerAPI api)
    {
        var serverCommand = api.ChatCommands.GetOrCreate("gantry");
        var subCommands = serverCommand.AllSubcommands;

        if (!subCommands.TryGetValue("", out _))
        {
            serverCommand
                .BeginSubCommand("")
                .EndSubCommand();
        }
    }
}