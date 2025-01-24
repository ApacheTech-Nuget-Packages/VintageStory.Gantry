namespace Gantry.Core.GameContent.ChatCommands.Parsers;

/// <summary>
///     Allows the user to search for an online player, based on a partial match of their username.
/// </summary>
/// <seealso cref="ArgumentParserBase" />
public class FuzzyPlayerParser(string argName, bool isMandatoryArg) : ArgumentParserBase(argName, isMandatoryArg)
{
    /// <summary>
    ///     The list of players whose username matches the <see cref="Value"/> search term.
    /// </summary>
    public List<IPlayer> Results { get; private set; } = [];

    /// <summary>
    ///     The search term to filter the list of players with.
    /// </summary>
    public string Value { get; private set; }

    /// <inheritdoc />
    public override void PreProcess(TextCommandCallingArgs args)
    {
        Value = null;
        base.PreProcess(args);
    }

    /// <inheritdoc />
    public override EnumParseResult TryProcess(TextCommandCallingArgs args, Action<AsyncParseResults> onReady = null)
    {
        Value = args.RawArgs.PopWord();
        if (string.IsNullOrEmpty(Value)) return EnumParseResult.Bad;
        Results = FuzzyPlayerSearch(Value).ToList();
        return EnumParseResult.Good;
    }

    /// <inheritdoc />
    public override object GetValue() => Value;

    /// <inheritdoc />
    public override void SetValue(object data)
    {
        Value = data.ToString();
        if (string.IsNullOrEmpty(Value)) return;
        Results = FuzzyPlayerSearch(Value).ToList();
    }

    private static IEnumerable<IPlayer> FuzzyPlayerSearch(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm)) return [];
        var players = ApiEx.ServerMain.AllPlayers;
        //if (players is null) return [];
        var results = new List<IPlayer>();
        foreach (var player in players)
        {
            if (player.PlayerName.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)) return [player];
            if (player.PlayerName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)) results.Add(player);
        }
        return results;
    }
}