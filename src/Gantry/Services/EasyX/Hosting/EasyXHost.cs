using Gantry.Core.GameContent.ChatCommands.Parsers;
using Gantry.Core.GameContent.ChatCommands.Parsers.Extensions;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Hosting;
using Vintagestory.API.Server;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace Gantry.Services.EasyX.Hosting;

/// <summary>
///     Entry-point for the mod. This class will configure and build the IOC Container, and Service list for the rest of the mod.
///     
///     Registrations performed within this class should be global scope; by convention, features should aim to be as stand-alone as they can be.
/// </summary>
/// <seealso cref="ModHost" />
[UsedImplicitly]
public abstract class EasyXHost(string _commandName) : ModHost
{
    private ConfigurationSettings _globalSettings = null!;

    ///<inheritdoc />
    protected override void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        services.AddFeatureGlobalSettings<ConfigurationSettings>();
        base.ConfigureServerModServices(services, sapi);
    }

    ///<inheritdoc />
    public override void StartServerSide(ICoreServerAPI api)
    {
        G.Logger.VerboseDebug($"Creating Chat Command: {_commandName}");

        _globalSettings = ModSettings.Global.Feature<ConfigurationSettings>();
        _globalSettings.CommandName = _commandName;
        ConfigurationSettings.Instance = _globalSettings;

        var command = api.ChatCommands.Create(_commandName)
            .RequiresPrivilege(Privilege.controlserver)
            .WithDescription(LangEx.EmbeddedFeatureString("EasyX", "ServerCommandDescription"));

        command
            .BeginSubCommand("scope")
            .WithArgs(api.ChatCommands.Parsers.FileScope())
            .WithDescription(LangEx.EmbeddedFeatureString("EasyX", "Scope.Description"))
            .HandleWith(OnChangeSettingsScope)
            .EndSubCommand();
    }

    private TextCommandResult OnChangeSettingsScope(TextCommandCallingArgs args)
    {
        var parser = args.Parsers[0].To<FileScopeParser>();

        if (parser.IsMissing)
        {
            var message = LangEx.EmbeddedFeatureString("EasyX", "Scope", _globalSettings.Scope.GetDescription());
            return TextCommandResult.Success(message);
        }

        if (parser.Scope is null)
        {
            const string validScopes = "[W]orld | [G]lobal";
            var invalidScopeMessage = LangEx.EmbeddedFeatureString("EasyX", "InvalidScope", validScopes);
            return TextCommandResult.Error(invalidScopeMessage);
        }

        var scope = parser.Scope!;

        var globalSettings = ModSettings.Global.Feature<ConfigurationSettings>();
        if (globalSettings.Scope != scope)
        {
            ModSettings.CopyTo(scope.Value);
            globalSettings.Scope = scope.Value;
            ModSettings.Global.Save(globalSettings);
        }

        var scopeMessage = LangEx.EmbeddedFeatureString("EasyX", "SetScope", _globalSettings.Scope.GetDescription());
        return TextCommandResult.Success(scopeMessage);
    }
}