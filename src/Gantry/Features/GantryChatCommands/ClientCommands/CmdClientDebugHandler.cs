using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Annotation;
using Gantry.Services.Brighter.Filters;

namespace Gantry.Features.GantryChatCommands.ClientCommands;

[ClientSide]
file class CmdClientDebugHandler : RequestHandler<CmdClientDebug>
{
    private readonly GantrySettings _gantrySettings;

    public CmdClientDebugHandler(GantrySettings gantrySettings)
    {
        _gantrySettings = gantrySettings;
    }


    [HandledOnClient]
    public override CmdClientDebug Handle(CmdClientDebug command)
    {
        var modIdParser = command.Args.Parsers[0];
        var modId = modIdParser.GetValue().To<string>();
        if (string.IsNullOrEmpty(modId))
        {
            command.Result = TextCommandResult.Error("Mod ID cannot be empty.");
            return base.Handle(command);
        }

        var stateParser = command.Args.Parsers[1];
        var state = stateParser.GetValue().To<bool>();

        return base.Handle(command);
    }
}