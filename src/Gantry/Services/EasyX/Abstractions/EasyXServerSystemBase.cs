using System.Linq.Expressions;
using System.Text;
using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.Extensions.DotNet;
using Gantry.Core.GameContent.ChatCommands.DataStructures;
using Gantry.Core.GameContent.ChatCommands.Parsers;
using Gantry.Core.GameContent.ChatCommands.Parsers.Extensions;
using Gantry.Core.Hosting.Registration;
using Gantry.Services.EasyX.Extensions;
using Gantry.Services.EasyX.Hosting;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Hosting;
using Gantry.Services.Network.Extensions;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

// ReSharper disable StringLiteralTypo

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Acts as a base class for all EasyX features, on the server.
/// </summary>
/// <typeparam name="TSettings">The type of the settings.</typeparam>
/// <typeparam name="TServerSettings">The type of the server settings.</typeparam>
/// <typeparam name="TClientSettings">The type of the client settings.</typeparam>
public abstract class EasyXServerSystemBase<TServerSettings, TClientSettings, TSettings> : ServerModSystem, IServerServiceRegistrar
    where TServerSettings : TSettings, IEasyXServerSettings, new()
    where TClientSettings : TSettings, IEasyXClientSettings, new()
    where TSettings : FeatureSettings<TServerSettings>, new()
{
    /// <summary>
    ///     
    /// </summary>
    protected IServerNetworkChannel? ServerChannel { get; private set; }

    /// <summary>
    ///     
    /// </summary>
    public static TServerSettings Settings
    {
        get
        {
            var settings = ModSettings.For(ConfigurationSettings.Instance.Scope);
            if (settings is null) return new TServerSettings();
            return settings.Feature<TServerSettings>() ?? new TServerSettings();
        }
    }

    /// <summary>
    /// Allows a mod to include Singleton, or Transient services to the IOC Container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="sapi">Access to the server-side API.</param>
    public void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        services.AddFeatureWorldSettings<TServerSettings>();
        services.AddFeatureGlobalSettings<TServerSettings>();
    }

    /// <summary>
    ///     
    /// </summary>
    protected abstract string SubCommandName { get; }

    /// <summary>
    ///     Translates a key into a string using the language system.
    /// </summary>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    protected string T(string path, params object[] args)
        => LangEx.FeatureString($"Easy{SubCommandName}", path, args);

    /// <summary>
    ///     
    /// </summary>
    protected virtual string CommandName => ConfigurationSettings.Instance.CommandName;

    /// <summary>
    ///     Adds feature specific sub-commands to the feature command. 
    /// </summary>
    /// <param name="subCommand">The sub-command to add features to.</param>
    /// <param name="parsers">The collection of parsers that can be used to parse command arguments.</param>
    protected virtual void FeatureSpecificCommands(IChatCommand subCommand, CommandArgumentParsers parsers)
    {
        // Do nothing, by default.
    }

    /// <summary>
    ///     Full start to the mod on the server side
    /// <br /><br />In 1.17+ do NOT use this to add or update behaviors or attributes or other fixed properties of any block, item or entity, in code (additional to what is read from JSON).
    /// It is already too late to do that here, it will not be seen client-side.  Instead, code which needs to do that should be registered for event sapi.Event.AssetsFinalizers.  See VSSurvivalMod system BlockReinforcement.cs for an example.
    /// </summary>
    /// <param name="api"></param>
    public override void StartServerSide(ICoreServerAPI api)
    {
        var parsers = api.ChatCommands.Parsers;

        var command = api.ChatCommands.GetOrCreate(CommandName)
            .BeginSubCommand(SubCommandName.ToLowerInvariant())
            .WithAlias(SubCommandName.ToLowerCaseInitials())
            .WithFeatureSpecifics(FeatureSpecificCommands, parsers)
            .HandleWith(DisplayInfo);

        command
            .BeginSubCommand("mode")
            .WithAlias("m")
            .WithArgs(parsers.AccessMode())
            .WithDescription(LangEx.Get("EasyX", "AccessMode.Description"))
            .HandleWith(OnChangeMode)
            .EndSubCommand();

        command
            .BeginSubCommand("whitelist")
            .WithAlias("wl")
            .WithArgs(parsers.OptionalServerPlayers())
            .WithDescription(LangEx.FeatureStringG("EasyX", "Whitelist.Description"))
            .HandleWith(HandleWhitelist)
            .EndSubCommand();

        command
            .BeginSubCommand("blacklist")
            .WithAlias("bl")
            .WithArgs(parsers.OptionalServerPlayers())
            .WithDescription(LangEx.FeatureStringG("EasyX", "Blacklist.Description"))
            .HandleWith(HandleBlacklist)
            .EndSubCommand();

        command.EndSubCommand();

        ServerChannel = api.Network
            .GetOrRegisterDefaultChannel()
            .RegisterMessageType<TClientSettings>();

        api.Event.PlayerJoin += player =>
        {
            var packet = GeneratePacket(player);
            G.Logger.VerboseDebug($"Sending {SubCommandName} Settings to {player.PlayerName}:");
            ServerChannel.SendPacket(packet, player);
        };
    }

    /// <summary>
    ///     Generates a packet, to send to the specified player.
    /// </summary>
    protected TClientSettings GeneratePacket(IPlayer player)
        => GeneratePacketPerPlayer(player, IsEnabledFor(player));

    /// <summary>
    ///     Generates a packet, to send to the specified player.
    /// </summary>
    protected virtual TClientSettings GeneratePacketPerPlayer(IPlayer player, bool isEnabled)
        => Settings.CreateFrom<TSettings, TClientSettings>().With(p => p.Enabled = isEnabled);

    /// <summary>
    ///     Determines whether this feature is enabled, for all the specified players.
    /// </summary>
    public static bool IsEnabledForAll(IEnumerable<string> players) => players.All(IsEnabledFor);

    /// <summary>
    ///     Determines whether this feature is enabled, for all the specified players.
    /// </summary>
    public static bool IsEnabledForAny(IEnumerable<string> players) => players.Any(IsEnabledFor);

    /// <summary>
    ///     Determines whether this feature is enabled, for the specified player.
    /// </summary>
    public static bool IsEnabledFor(IPlayer player) => IsEnabledFor(player.PlayerUID);

    /// <summary>
    ///     Determines whether this feature is enabled, for the specified player.
    /// </summary>
    private static bool IsEnabledFor(string playerUID)
    {
        return Settings.Mode switch
        {
            AccessMode.Disabled => false,
            AccessMode.Enabled => true,
            AccessMode.Whitelist => Settings.Whitelist.Select(p => p.Id).Any(p => p == playerUID),
            AccessMode.Blacklist => Settings.Blacklist.Select(p => p.Id).All(p => p != playerUID),
            _ => throw new ArgumentOutOfRangeException(nameof(playerUID))
        };
    }

    /// <summary>
    ///     Call Handler: /ChatCommandName (X)
    /// </summary>
    protected virtual TextCommandResult DisplayInfo(TextCommandCallingArgs args)
    {
        var sb = new StringBuilder();
        sb.AppendLine(LangEx.FeatureStringG("EasyX", "Mode", Lang.Get(SubCommandName.SplitPascalCase().UcFirst()), Settings.Mode));
        ExtraDisplayInfo(sb);
        return TextCommandResult.Success(sb.ToString().TrimEnd(Environment.NewLine.ToCharArray()));
    }

    /// <summary>
    ///     Add extra info to the details of the sub command.
    /// </summary>
    /// <param name="sb"></param>
    protected virtual void ExtraDisplayInfo(StringBuilder sb)
    {
        // Do nothing, by default.
    }

    /// <summary>
    ///     Call Handler: /ChatCommandName (X) mode
    /// </summary>
    protected virtual TextCommandResult OnChangeMode(TextCommandCallingArgs args)
    {
        var mode = args.Parsers[0].To<AccessModeParser>().Mode;
        if (mode is null)
        {
            const string validModes = "[D]isabled | [E]nabled | [W]hitelist | [B]lacklist]";
            var invalidModeMessage = LangEx.FeatureStringG("EasyX", "InvalidMode", validModes);
            return TextCommandResult.Error(invalidModeMessage);
        }

        Settings.Mode = mode.Value;
        var modeMessage = LangEx.FeatureStringG("EasyX", "SetMode", SubCommandName, Settings.Mode);
        ServerChannel?.BroadcastUniquePacket(GeneratePacket);
        return TextCommandResult.Success(modeMessage);
    }

    /// <summary>
    ///     Call Handler: /ChatCommandName (X) whitelist
    /// </summary>
    protected virtual TextCommandResult HandleWhitelist(TextCommandCallingArgs args)
        => HandleListChange(args, s => s.Whitelist);

    /// <summary>
    ///     Call Handler: /ChatCommandName (X) blacklist
    /// </summary>
    protected virtual TextCommandResult HandleBlacklist(TextCommandCallingArgs args)
        => HandleListChange(args, s => s.Blacklist);

    private TextCommandResult HandleListChange<TProperty>(TextCommandCallingArgs args, Expression<System.Func<TServerSettings, TProperty>> property)
        where TProperty : ICollection<Player>
    {
        if (property.Body is not MemberExpression memberExpression)
            throw new ArgumentException("The provided expression does not represent a property.");
        var propertyName = memberExpression.Member.Name;
        var list = Settings.GetProperty<TProperty>(propertyName);

        var parser = args.Parsers[0].To<GantryPlayersArgParser>();
        if (!parser.IsMissing)
        {
            var searchTerm = parser.SearchTerm;
            var players = parser.GetValue().To<PlayerUidName[]>();
            return ProcessParserResults(searchTerm, players, list, propertyName);
        }

        var sb = new StringBuilder();
        var resultCount = list.Count > 0 ? "Results" : "NoResults";
        sb.AppendLine(LangEx.FeatureStringG("EasyX", $"{propertyName}.{resultCount}", SubCommandName));
        foreach (var p in list)
        {
            sb.AppendLine($" - {p.Name} (PID: {p.Id})");
        }
        return TextCommandResult.Success(sb.ToString());
    }

    /// <summary>
    ///     Base Call Handler
    /// </summary>
    protected TextCommandResult OnChange<T>(TextCommandCallingArgs args, string propertyName, Action<T?>? validate = null)
    {
        var value = (args.Parsers[0].GetValue().To<T>() ?? default).With(validate);
        Settings.SetProperty(propertyName, value);
        var message = LangEx.FeatureString(SubCommandName, propertyName, value);
        ServerChannel?.BroadcastUniquePacket(GeneratePacket);
        return TextCommandResult.Success(message);
    }

    private TextCommandResult ProcessParserResults(string searchTerm, PlayerUidName[] players, ICollection<Player> list, string listType)
    {
        var message = players.Length switch
        {
            1 => FoundSinglePlayer(list, listType, players),
            > 1 => FoundMultiplePlayers(searchTerm, players),
            _ => FoundNoResults(searchTerm)
        };
        return message;
    }

    /// <summary>
    ///     Processes a single server player by either removing an existing player from the list or adding a new one,
    ///     then returns a message indicating the action performed.
    /// </summary>
    /// <param name="list">The collection of players to update.</param>
    /// <param name="listType">The type of list, used for localisation of the message.</param>
    /// <param name="players">The list of server players; the first entry is used for processing.</param>
    /// <returns>
    ///     A string message indicating whether a player was added or removed.
    /// </returns>
    private TextCommandResult FoundSinglePlayer(ICollection<Player> list, string listType, PlayerUidName[] players)
    {
        var result = players.First();
        var existingPlayer = list.SingleOrDefault(p => p.Id == result.Uid);
        var isRemoved = existingPlayer is not null && list.Remove(existingPlayer);
        if (!isRemoved) list.Add(result!);
        ModSettings.World.Save(Settings);
        ServerChannel?.BroadcastUniquePacket(GeneratePacket);
        var message = LangEx.FeatureStringG("EasyX", $"{listType}.{(isRemoved ? "PlayerRemoved" : "PlayerAdded")}", result.Name, SubCommandName);
        return TextCommandResult.Success(message);
    }

    private static TextCommandResult FoundNoResults(string searchTerm)
        => TextCommandResult.Error(LangEx.FeatureStringG("EasyX", "PlayerSearch.NoResults", searchTerm));

    private static TextCommandResult FoundMultiplePlayers(string searchTerm, PlayerUidName[] players)
    {
        var sb = new StringBuilder();
        sb.Append(LangEx.FeatureStringG("EasyX", "PlayerSearch.MultipleResults", searchTerm));
        foreach (var p in players)
        {
            sb.Append($" - {p.Name} (PID: {p.Uid})");
        }
        return TextCommandResult.Error(sb.ToString());
    }
}