using System.Reflection;
using System.Text;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Gantry.Core.Extensions.DotNet;
using Gantry.Core.Hosting;
using Gantry.Core.Hosting.Registration;
using Gantry.Core.ModSystems;
using Gantry.Services.EasyX.ChatCommands.DataStructures;
using Gantry.Services.EasyX.ChatCommands.Parsers;
using Gantry.Services.EasyX.ChatCommands.Parsers.Extensions;
using Gantry.Services.EasyX.Extensions;
using Gantry.Services.EasyX.Hosting;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Configuration.Abstractions;
using Gantry.Services.FileSystem.Hosting;
using Gantry.Services.Network;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Server;

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
    where TSettings : FeatureSettings
{
    /// <summary>
    ///     
    /// </summary>
    protected IServerNetworkChannel ServerChannel;

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
            .WithDescription(LangEx.EmbeddedFeatureString("EasyX", "AccessMode.Description"))
            .HandleWith(OnChangeMode)
            .EndSubCommand();

        command
            .BeginSubCommand("whitelist")
            .WithAlias("wl")
            .WithArgs(parsers.FuzzyPlayerSearch())
            .WithDescription(LangEx.EmbeddedFeatureString("EasyX", "Whitelist.Description"))
            .HandleWith(HandleWhitelist)
            .EndSubCommand();

        command
            .BeginSubCommand("blacklist")
            .WithAlias("bl")
            .WithArgs(parsers.FuzzyPlayerSearch())
            .WithDescription(LangEx.EmbeddedFeatureString("EasyX", "Blacklist.Description"))
            .HandleWith(HandleBlacklist)
            .EndSubCommand();

        command.EndSubCommand();

        ServerChannel = IOC.Services.Resolve<IServerNetworkService>()
            .DefaultServerChannel
            .RegisterMessageType<TClientSettings>();

        api.Event.PlayerJoin += player =>
        {
            var packet = GeneratePacket(player);
            api.Logger.GantryDebug($"Sending {SubCommandName} Settings to {player.PlayerName}:");
            api.Logger.GantryDebug(packet.ToXml());
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
        sb.AppendLine(LangEx.EmbeddedFeatureString("EasyX", "Mode", Lang.Get(SubCommandName.SplitPascalCase().UcFirst()), Settings.Mode));
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
            var invalidModeMessage = LangEx.EmbeddedFeatureString("EasyX", "InvalidMode", validModes);
            return TextCommandResult.Error(invalidModeMessage);
        }

        Settings.Mode = mode.Value;
        var modeMessage = LangEx.EmbeddedFeatureString("EasyX", "SetMode", SubCommandName, Settings.Mode);
        ServerChannel?.BroadcastUniquePacket(GeneratePacket);
        return TextCommandResult.Success(modeMessage);
    }

    /// <summary>
    ///     Call Handler: /ChatCommandName (X) whitelist
    /// </summary>
    protected virtual TextCommandResult HandleWhitelist(TextCommandCallingArgs args)
    {
        var parser = args.Parsers[0].To<FuzzyPlayerParser>();
        if (parser.Value is not null)
        {
            var message = AddRemovePlayerFromList(parser, Settings.Whitelist, "Whitelist");
            return TextCommandResult.Success(message);
        }

        var sb = new StringBuilder();
        var resultCount = Settings.Whitelist.Count > 0 ? "Results" : "NoResults";
        sb.AppendLine(LangEx.EmbeddedFeatureString("EasyX", $"Whitelist.{resultCount}", SubCommandName));
        foreach (var p in Settings.Whitelist)
        {
            sb.AppendLine($" - {p.Name} (PID: {p.Id})");
        }
        return TextCommandResult.Success(sb.ToString());
    }

    /// <summary>
    ///     Call Handler: /ChatCommandName (X) blacklist
    /// </summary>
    protected virtual TextCommandResult HandleBlacklist(TextCommandCallingArgs args)
    {
        var parser = args.Parsers[0].To<FuzzyPlayerParser>();
        if (parser.Value is not null)
        {
            var message = AddRemovePlayerFromList(parser, Settings.Blacklist, "Blacklist");
            return TextCommandResult.Success(message);
        }

        var sb = new StringBuilder();
        var resultCount = Settings.Blacklist.Count > 0 ? "Results" : "NoResults";
        sb.AppendLine(LangEx.EmbeddedFeatureString("EasyX", $"Blacklist.{resultCount}", SubCommandName));
        foreach (var p in Settings.Blacklist)
        {
            sb.AppendLine($" - {p.Name} (PID: {p.Id})");
        }
        return TextCommandResult.Success(sb.ToString());
    }

    /// <summary>
    ///     Base Call Handler
    /// </summary>
    protected TextCommandResult OnChange<T>(TextCommandCallingArgs args, string propertyName, Action<T> validate = null)
    {
        var value = (args.Parsers[0].GetValue().To<T>() ?? default).With(validate);
        Settings.SetProperty(propertyName, value);
        var message = LangEx.FeatureString(SubCommandName, propertyName, value);
        ServerChannel?.BroadcastUniquePacket(GeneratePacket);
        return TextCommandResult.Success(message);
    }

    private string AddRemovePlayerFromList(FuzzyPlayerParser parser, ICollection<Player> list, string listType)
    {
        var players = parser.Results;
        var searchTerm = parser.Value;

        var message = players.Count switch
        {
            1 => FoundSinglePlayer(list, listType, players),
            > 1 => FoundMultiplePlayers(searchTerm, players),
            _ => FoundNoResults(searchTerm)
        };
        ServerChannel?.BroadcastUniquePacket(GeneratePacket);
        return message;
    }

    private string FoundSinglePlayer(ICollection<Player> list, string listType, List<IPlayer> players)
    {
        var result = players.First();
        var plr = list.SingleOrDefault(p => p.Id == result.PlayerUID);
        if (plr is not null)
        {
            list.Remove(plr);
            return LangEx.EmbeddedFeatureString("EasyX", $"{listType}.PlayerRemoved", result.PlayerName, SubCommandName);
        }

        list.Add(new Player(result.PlayerUID, result.PlayerName));
        ModSettings.World.Save(Settings);

        return LangEx.EmbeddedFeatureString("EasyX", $"{listType}.PlayerAdded", result.PlayerName, SubCommandName);
    }

    private static string FoundNoResults(string searchTerm)
        => LangEx.EmbeddedFeatureString("EasyX", "PlayerSearch.NoResults", searchTerm);

    private static string FoundMultiplePlayers(string searchTerm, List<IPlayer> players)
    {
        var sb = new StringBuilder();
        sb.Append(LangEx.EmbeddedFeatureString("EasyX", "PlayerSearch.MultipleResults", searchTerm));
        foreach (var p in players)
        {
            sb.Append($" - {p.PlayerName} (PID: {p.PlayerUID})");
        }
        return sb.ToString();
    }
}