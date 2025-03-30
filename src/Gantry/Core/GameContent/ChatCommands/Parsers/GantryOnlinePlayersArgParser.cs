namespace Gantry.Core.GameContent.ChatCommands.Parsers;

/// <summary>
///     Provides an argument parser for players that are active on the server.
/// </summary>
/// <param name="argName">The name of the argument.</param>
/// <param name="api">The core API instance.</param>
/// <param name="isMandatoryArg">Indicates whether the argument is mandatory.</param>
public class GantryOnlinePlayersArgParser(string argName, ICoreAPI api, bool isMandatoryArg)
    : OnlinePlayerArgParser(argName, api, isMandatoryArg)
{
    /// <summary>
    ///     Gets the search term used for filtering players.
    /// </summary>
    public string SearchTerm { get; private set; } = string.Empty;

    /// <inheritdoc />
    public override void PreProcess(TextCommandCallingArgs args)
    {
        SearchTerm = args.RawArgs.PeekWord();
        base.PreProcess(args);
    }
}