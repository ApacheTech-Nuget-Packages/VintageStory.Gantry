using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Annotation;
using Gantry.Services.Brighter.Filters;
using Gantry.Services.BrighterChat;

namespace Gantry.Features.GantryChatCommands.ServerCommands;

[ServerSide]
internal class CmdServerDebug : MediatedChatCommand
{
    [ServerSide]
    internal class Handler : RequestHandler<CmdServerDebug>
    {
        public Handler()
        {

        }

        [HandledOnServer]
        public override CmdServerDebug Handle(CmdServerDebug command)
        {
            return base.Handle(command);
        }
    }
}