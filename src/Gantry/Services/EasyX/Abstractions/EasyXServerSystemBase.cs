using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.Abstractions;
using Gantry.Core.Abstractions.ModSystems;
using Gantry.Core.Annotation;
using Gantry.Core.Hosting.Registration;
using Gantry.Core.Network.Extensions;
using Gantry.Extensions.Api;
using Gantry.GameContent.ChatCommands.DataStructures;
using Gantry.GameContent.ChatCommands.Parsers;
using Gantry.GameContent.ChatCommands.Parsers.Extensions;
using Gantry.Services.EasyX.Extensions;
using Gantry.Services.IO.Configuration.Abstractions;
using Gantry.Services.IO.DataStructures;
using Gantry.Services.IO.Hosting;
using System.Linq.Expressions;
using System.Text;
using Vintagestory.API.Util;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Acts as a base class for all EasyX features, on the server.
/// </summary>
public abstract class EasyXServerSystemBase<TModSystem, TServerSettings, TClientSettings> 
    : ServerModSystem<TModSystem>, IServerServiceRegistrar
    where TModSystem : EasyXServerSystemBase<TModSystem, TServerSettings, TClientSettings>
    where TServerSettings : FeatureSettings<TServerSettings>, IEasyXServerSettings, new()
    where TClientSettings : class, IEasyXClientSettings, new()
{
    /// <summary>
    ///     The server network channel for this feature.
    /// </summary>
    protected IServerNetworkChannel? ServerChannel { get; private set; }

    /// <summary>
    ///     The configuration settings for the mod.
    /// </summary>
    public ConfigurationSettings Configuration => Core.Settings.Global.Feature<ConfigurationSettings>();

    /// <summary>
    ///     The scope at which this feature's settings are applied.
    /// </summary>
    protected ModFileScope Scope => Configuration.Scope;

    /// <summary>
    ///     The settings for this feature.
    /// </summary>
    public TServerSettings Settings
    {
        get
        {
            var settings = Core.Settings.For(Scope);
            return settings.Feature<TServerSettings>() ?? new TServerSettings();
        }
    }

    /// <summary>
    ///     Allows a mod to include Singleton, or Transient services to the IOC Container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="gantry"> The Gantry API, which provides access to the mod's context and services.</param>
    public void ConfigureServerModServices(IServiceCollection services, ICoreGantryAPI gantry)
    {
        services.AddScopedFeatureSettings<TServerSettings>();
    }

    /// <summary>
    ///     The name of the sub-command, used to access this feature's commands.
    /// </summary>
    protected abstract string SubCommandName { get; }

    /// <summary>
    ///     Translates a key into a string using the language system.
    /// </summary>
    /// <param name="path">The path to the feature based string to translate.</param>
    /// <param name="args">The arguments to pass to the lang file.</param>
    protected string T(string path, params object[] args)
        => Core.Lang.Translate($"{SubCommandName}", path, args);

    /// <summary>
    ///     The name of the main chat command, used to access all EasyX features.
    /// </summary>
    protected virtual string CommandName 
        => Core.Settings.Global.Feature<ConfigurationSettings>().CommandName;

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
            .WithDescription(Core.Lang.Translate(SubCommandName, "Description"))
            .WithFeatureSettings<TServerSettings>(SubCommandName, parsers, Core.Lang, OnChange)
            .WithFeatureSpecifics(FeatureSpecificCommands, parsers)
            .HandleWith(DisplayInfo);

        command
            .BeginSubCommand("mode")
            .WithAlias("m")
            .WithArgs(parsers.AccessMode())
            .WithDescription(Core.Lang.Get("EasyX", "AccessMode.Description"))
            .HandleWith(OnChangeMode)
            .EndSubCommand();

        command
            .BeginSubCommand("whitelist")
            .WithAlias("wl")
            .WithArgs(parsers.OptionalServerPlayers(api))
            .WithDescription(Core.Lang.TranslateG("EasyX", "Whitelist.Description"))
            .HandleWith(HandleWhitelist)
            .EndSubCommand();

        command
            .BeginSubCommand("blacklist")
            .WithAlias("bl")
            .WithArgs(parsers.OptionalServerPlayers(api))
            .WithDescription(Core.Lang.TranslateG("EasyX", "Blacklist.Description"))
            .HandleWith(HandleBlacklist)
            .EndSubCommand();

        command.EndSubCommand();

        ServerChannel = api.Network
            .GetOrRegisterDefaultChannel(Core)
            .RegisterPacket<SettingsSavedPacket>(Core)
            .RegisterPacket<TServerSettings>(Core, OnSettingsChanged)
            .RegisterPacket<TClientSettings>(Core);

        api.Event.PlayerJoin += player =>
        {
            var packet = GeneratePacket(player);
            Core.Logger.VerboseDebug($"Sending {SubCommandName} Settings to {player.PlayerName}:");
            ServerChannel.SendPacket(packet, player);
            ServerChannel.SendPacket(Settings, player);
        };
    }

    private void OnSettingsChanged(IServerPlayer player, TServerSettings packet)
    {
        if (!player.Privileges.Contains(Privilege.controlserver)) return;
        Settings.UpdateSettings(packet);
        ServerChannel?.BroadcastPacket(Settings);
        ServerChannel?.BroadcastUniquePacket(Sapi.AsServerMain(), GeneratePacket);
        ServerChannel?.SendPacket(new SettingsSavedPacket { FeatureName = SubCommandName }, player);
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
        => Settings.MapTo<TServerSettings, TClientSettings>().With(p => p.Enabled = isEnabled);

    /// <summary>
    ///     Determines whether this feature is enabled, for all the specified players.
    /// </summary>
    public bool IsEnabledForAll(IEnumerable<string> players) => players.All(IsEnabledFor);

    /// <summary>
    ///     Determines whether this feature is enabled, for all the specified players.
    /// </summary>
    public bool IsEnabledForAny(IEnumerable<string> players) => players.Any(IsEnabledFor);

    /// <summary>
    ///     Determines whether this feature is enabled, for the specified player.
    /// </summary>
    public bool IsEnabledFor(IPlayer player) => IsEnabledFor(player.PlayerUID);

    /// <summary>
    ///     Determines whether this feature is enabled, for the specified player.
    /// </summary>
    private bool IsEnabledFor(string playerUID)
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
        sb.AppendLine(Core.Lang.TranslateG("EasyX", "Mode", Lang.Get(SubCommandName.SplitPascalCase().UcFirst()), Settings.Mode));
        ExtraDisplayInfo(sb);
        return TextCommandResult.Success(sb.ToString().TrimEnd(Environment.NewLine.ToCharArray()));
    }

    /// <summary>
    ///     Add extra info to the details of the sub command.
    /// </summary>
    /// <param name="sb"></param>
    protected virtual void ExtraDisplayInfo(StringBuilder sb)
    {
        foreach (var property in typeof(TServerSettings).GetProperties())
        {
            if (property.GetCustomAttributes(typeof(ChatCommandAttribute), true).FirstOrDefault() is not ChatCommandAttribute) continue;
            var value = property.GetValue(Settings);
            sb.AppendLine(Core.Lang.Translate(SubCommandName, property.Name, value));
        }
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
            var invalidModeMessage = Core.Lang.TranslateG("EasyX", "InvalidMode", validModes);
            return TextCommandResult.Error(invalidModeMessage);
        }

        Settings.Mode = mode.Value;
        var modeMessage = Core.Lang.TranslateG("EasyX", "SetMode", SubCommandName, Settings.Mode);
        ServerChannel?.BroadcastUniquePacket(Sapi.AsServerMain(), GeneratePacket);
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
        var list = Settings.GetProperty<TProperty>(propertyName) 
            ?? throw new InvalidOperationException($"The property '{propertyName}' is null or not a collection.");
        var parser = args.Parsers[0].To<GantryPlayersArgParser>();
        if (!parser.IsMissing)
        {
            var searchTerm = parser.SearchTerm;
            var players = parser.GetValue().To<PlayerUidName[]>();
            return ProcessParserResults(searchTerm, players, list, propertyName);
        }

        var sb = new StringBuilder();
        var resultCount = list.Count > 0 ? "Results" : "NoResults";
        sb.AppendLine(Core.Lang.TranslateG("EasyX", $"{propertyName}.{resultCount}", SubCommandName));
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
        if (value is null)
        {
            var errorMessage = Core.Lang.Translate("EasyX", "InvalidValue", propertyName);
            return TextCommandResult.Error(errorMessage);
        }
        Settings.SetProperty(propertyName, value);
        var message = Core.Lang.Translate(SubCommandName, propertyName, value);
        ServerChannel?.BroadcastUniquePacket(Sapi.AsServerMain(), GeneratePacket);
        return TextCommandResult.Success(message);
    }

    /// <summary>
    ///     Base Call Handler
    /// </summary>
    protected TextCommandResult OnChange(TextCommandCallingArgs args, string propertyName)
    {
        var value = args.Parsers[0].GetValue();
        if (value is null)
        {
            var errorMessage = Core.Lang.Translate("EasyX", "InvalidValue", propertyName);
            return TextCommandResult.Error(errorMessage);
        }
        Settings.SetProperty(propertyName, value);
        var message = Core.Lang.Translate(SubCommandName, propertyName, value);
        ServerChannel?.BroadcastUniquePacket(Sapi.AsServerMain(), GeneratePacket);
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
    [ServerSide]
    private TextCommandResult FoundSinglePlayer(ICollection<Player> list, string listType, PlayerUidName[] players)
    {
        var result = players.First();
        var existingPlayer = list.SingleOrDefault(p => p.Id == result.Uid);
        var isRemoved = existingPlayer is not null && list.Remove(existingPlayer);
        if (!isRemoved) list.Add(result!);
        Core.Settings.World.Save(Settings);
        ServerChannel?.BroadcastUniquePacket(Sapi.AsServerMain(), GeneratePacket);
        var message = Core.Lang.TranslateG("EasyX", $"{listType}.{(isRemoved ? "PlayerRemoved" : "PlayerAdded")}", result.Name, SubCommandName);
        return TextCommandResult.Success(message);
    }

    private TextCommandResult FoundNoResults(string searchTerm)
        => TextCommandResult.Error(Core.Lang.TranslateG("EasyX", "PlayerSearch.NoResults", searchTerm));

    private TextCommandResult FoundMultiplePlayers(string searchTerm, PlayerUidName[] players)
    {
        var sb = new StringBuilder();
        sb.Append(Core.Lang.TranslateG("EasyX", "PlayerSearch.MultipleResults", searchTerm));
        foreach (var p in players)
        {
            sb.Append($" - {p.Name} (PID: {p.Uid})");
        }
        return TextCommandResult.Error(sb.ToString());
    }
}