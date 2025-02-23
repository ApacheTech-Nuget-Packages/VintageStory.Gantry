using Gantry.Core.Annotation;
using Gantry.Core.Maths.FuzzyLogic;

namespace Gantry.Core.GameContent.ChatCommands.Parsers;

/// <summary>
///     Allows the user to search for an online player, based on a partial match of their username.
///     Only works server-side.
/// </summary>
/// <seealso cref="ArgumentParserBase" />
[ServerSide]
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
        var value = args.RawArgs.PopWord();
        if (string.IsNullOrEmpty(value)) return EnumParseResult.Bad;
        SetValue(value);
        return EnumParseResult.Good;
    }

    /// <inheritdoc />
    public override object GetValue() => Value;

    /// <inheritdoc />
    public override void SetValue(object data)
    {
        Value = data.ToString();
        if (string.IsNullOrEmpty(Value)) return;
        Results = FuzzyPlayerSearch(Value);
    }

    private static List<IPlayer> FuzzyPlayerSearch(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm)) return [];

        var dictionary = ApiEx.ServerMain.PlayersByUid;

        var players = ApiEx.ServerMain.AllPlayers;
        if (players is null) return [];

        var results = new List<(IPlayer player, int distance)>();

        foreach (var player in players)
        {
            var distance = player.PlayerName.LevenshteinDistance(searchTerm);
            if (distance == 0) return [player];
            results.Add((player, distance));
        }

        return [.. results.OrderBy(r => r.distance).Select(r => r.player)];
    }
}