namespace Gantry.Core.GameContent.ChatCommands.Parsers;

/// <inheritdoc />
public class GantryPlayersArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : PlayersArgParser(argName, api, isMandatoryArg)
{
    /// <summary>
    /// 
    /// </summary>
    public string SearchTerm { get; private set; }

    /// <inheritdoc />
    public override void PreProcess(TextCommandCallingArgs args)
    {
        SearchTerm = args.RawArgs.PeekWord();
        base.PreProcess(args);
    }
}