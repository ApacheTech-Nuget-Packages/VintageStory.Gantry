using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.DependencyInjection;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Settings;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo

namespace Gantry.Tests.AcceptanceMod.Features.FileSystem
{
    internal sealed class FileSystemClientProgram : ClientModSystem
    {
        private IFileSystemService _fileSystemService;

        private IMessageProvider _worldSettings;
        private IMessageProvider _globalSettings;

        private IMessageProvider _embeddedWorldSettings;
        private IMessageProvider _embeddedGlobalSettings;

        public override void StartClientSide(ICoreClientAPI api)
        {
            _fileSystemService = IOC.Services.Resolve<IFileSystemService>();
            _worldSettings = ModSettings.World.Feature<ClientWorldSettings>();
            _globalSettings = ModSettings.Global.Feature<ClientGlobalSettings>();
            _embeddedWorldSettings = _fileSystemService.ParseEmbeddedJsonFile<EmbeddedJsonSettings>("embedded-world-client.json");
            _embeddedGlobalSettings = _fileSystemService.ParseEmbeddedJsonFile<EmbeddedJsonSettings>("embedded-global-client.json");

            Capi.ChatCommands
                .Create()
                .WithName("filesystemtest")
                .WithDescription("Test the Gantry File System Service.")
                .HandleWith(a =>
                {
                    Handler(a.Caller.FromChatGroupId, a.RawArgs);
                    return TextCommandResult.Success();
                });
        }

        private void Handler(int groupId, CmdArgs args)
        {
            var scope = args.PopWord("world");
            var type = args.PopWord("file");
            var provider = type switch
            {
                "file" when scope is "world" => _worldSettings,
                "file" when scope is "global" => _globalSettings,
                "embedded" when scope is "world" => _embeddedWorldSettings,
                "embedded" when scope is "global" => _embeddedGlobalSettings,
                _ => _worldSettings
            };
            Capi.ShowChatMessage(provider.Message);
        }
    }
}
