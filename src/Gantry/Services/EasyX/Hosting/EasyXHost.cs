using Gantry.Core.Abstractions;
using Gantry.Core.Hosting.Registration;
using Gantry.GameContent.ChatCommands.Parsers;
using Gantry.GameContent.ChatCommands.Parsers.Extensions;
using Gantry.Services.IO.Hosting;

namespace Gantry.Services.EasyX.Hosting;

/// <summary>
///     Entry-point for the mod. This class will configure and build the IOC Container, and Service list for the rest of the mod.
///     
///     Registrations performed within this class should be global scope; by convention, features should aim to be as stand-alone as they can be.
/// </summary>
/// <seealso cref="ModHost{TModSystem}" />
public abstract class EasyXHost<TModSystem>(string commandName) 
    : ModHost<TModSystem>(), IServerServiceRegistrar where TModSystem : ModHost<TModSystem>
{
    private readonly string _commandName = commandName;
    private ConfigurationSettings _globalSettings = null!;

    /// <inheritdoc />
    public void ConfigureServerModServices(IServiceCollection services, ICoreGantryAPI gantry)
    {
        services.AddFeatureGlobalSettings<ConfigurationSettings>();
    }

    /// <inheritdoc />
    public override void StartServerSide(ICoreServerAPI api)
    {
        Core.Logger.VerboseDebug($"Creating Chat Command: {_commandName}");

        _globalSettings = Core.Settings.Global.Feature<ConfigurationSettings>();
        _globalSettings.CommandName = _commandName;

        var command = api.ChatCommands.Create(_commandName)
            .RequiresPrivilege(Privilege.controlserver)
            .WithDescription(Core.Lang.TranslateG("EasyX", "ServerCommandDescription"));

        command
            .BeginSubCommand("scope")
            .WithArgs(api.ChatCommands.Parsers.FileScope())
            .WithDescription(Core.Lang.TranslateG("EasyX", "Scope.Description"))
            .HandleWith(OnChangeSettingsScope)
            .EndSubCommand();
    }

    private TextCommandResult OnChangeSettingsScope(TextCommandCallingArgs args)
    {
        var parser = args.Parsers[0].To<FileScopeParser>();

        if (parser.IsMissing)
        {
            var message = Core.Lang.TranslateG("EasyX", "Scope", _globalSettings.Scope.GetDescription());
            return TextCommandResult.Success(message);
        }

        if (!parser.Scope.HasValue)
        {
            const string validScopes = "[W]orld | [G]lobal";
            var invalidScopeMessage = Core.Lang.TranslateG("EasyX", "InvalidScope", validScopes);
            return TextCommandResult.Error(invalidScopeMessage);
        }

        if (parser.Scope is IO.DataStructures.ModFileScope.Gantry)
        {
            const string gantryScopeMessage = "The Gantry scope is reserved for the Gantry MDK and cannot be set.";
            return TextCommandResult.Error(gantryScopeMessage);
        }

        var toScope = parser.Scope.Value;

        var fromScope = _globalSettings.Scope;
        if (fromScope != toScope)
        {
            Core.Settings.CopySettings(fromScope, toScope);
            _globalSettings.Scope = toScope;
        }

        var scopeMessage = Core.Lang.TranslateG("EasyX", "SetScope", _globalSettings.Scope.GetDescription());
        return TextCommandResult.Success(scopeMessage);
    }
}