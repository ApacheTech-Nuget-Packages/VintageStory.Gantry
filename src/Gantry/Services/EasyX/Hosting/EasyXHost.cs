using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Gantry.Core.Hosting;
using Gantry.Services.EasyX.ChatCommands.Parsers;
using Gantry.Services.EasyX.ChatCommands.Parsers.Extensions;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Services.FileSystem.Hosting;
using Gantry.Services.HarmonyPatches.Hosting;
using Gantry.Services.Network.Hosting;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace Gantry.Services.EasyX.Hosting;

/// <summary>
///     Entry-point for the mod. This class will configure and build the IOC Container, and Service list for the rest of the mod.
///     
///     Registrations performed within this class should be global scope; by convention, features should aim to be as stand-alone as they can be.
/// </summary>
/// <remarks>
///     Only one derived instance of this class should be added to any single mod within
///     the VintageMods domain. This class will enable Dependency Injection, and add all
///     the domain services. Derived instances should only have minimal functionality, 
///     instantiating, and adding Application specific services to the IOC Container.
/// </remarks>
/// <seealso cref="ModHost" />
[UsedImplicitly]
public abstract class EasyXHost(string _commandName) : ModHost(debugMode:
#if DEBUG
    true
#else
    false
#endif
)
{
    private ConfigurationSettings _globalSettings;

    ///<inheritdoc />
    protected override void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        sapi.Logger.GantryDebug($"[{ModEx.ModInfo.Name}] Adding FileSystem Service");
        services.AddFileSystemService(sapi, o => o.RegisterSettingsFiles = true);
        services.AddFeatureGlobalSettings<ConfigurationSettings>();
    }

    ///<inheritdoc />
    protected override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
    {
        api.Logger.GantryDebug($"[{ModEx.ModInfo.Name}] Adding Harmony Service");
        services.AddHarmonyPatchingService(api, o => o.AutoPatchModAssembly = true);

        api.Logger.GantryDebug($"[{ModEx.ModInfo.Name}] Adding Network Service");
        services.AddNetworkService(api);
    }

    ///<inheritdoc />
    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Logger.GantryDebug($"[{ModEx.ModInfo.Name}] Create Chat Command: {_commandName}");

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
        }

        var scopeMessage = LangEx.EmbeddedFeatureString("EasyX", "SetScope", _globalSettings.Scope.GetDescription());
        return TextCommandResult.Success(scopeMessage);
    }
}