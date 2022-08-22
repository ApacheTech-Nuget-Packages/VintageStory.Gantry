using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.Common.Extensions.System;
using Gantry.Core;
using Gantry.Core.DependencyInjection;
using Gantry.Core.ModSystems;
using Gantry.Services.Network;
using Gantry.Tests.AcceptanceMod.Features.Network.Packets;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable StringLiteralTypo

namespace Gantry.Tests.AcceptanceMod.Features.Network
{
    internal sealed class NetworkProgram : UniversalModSystem
    {
        private IUniversalNetworkService _networkService;
        private IClientNetworkChannel _clientChannel;
        private IServerNetworkChannel _serverChannel;

        public override void Start(ICoreAPI api) 
            => _networkService = IOC.Services.Resolve<IUniversalNetworkService>();

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            _serverChannel = _networkService.DefaultServerChannel
                .RegisterMessageType<TestMessagePacket>()
                .SetMessageHandler<TestMessagePacket>((fromPlayer, packet)
                    => _serverChannel.SendPacket(
                    packet.With(p => p.Message = $"Player Name: {fromPlayer.PlayerName}\n" +
                                                 $"Message: {packet.Message}"),
                    fromPlayer));
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            _clientChannel = _networkService.DefaultClientChannel
                .RegisterMessageType<TestMessagePacket>()
                .SetMessageHandler<TestMessagePacket>(
                    p => ApiEx.Client.ShowChatMessage(p.Message));

            Capi.RegisterCommand(
                "testmessage",
                "Sends a test message packet to the server", "[message]",
                (_, _) => _clientChannel.SendPacket(new TestMessagePacket { Message = "Test Successful." }));
        }
    }
}
