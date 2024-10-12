using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core;
using Gantry.Core.Extensions.Api;
using Gantry.Core.Extensions.DotNet;
using Gantry.Core.Hosting;
using Gantry.Core.ModSystems;
using Gantry.Services.Network;
using Vintagestory.API.Client;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     
/// </summary>
public abstract class EasyXClientSystemBase<TClientSettings> : ClientModSystem
    where TClientSettings : class, IEasyXClientSettings, new()
{
    /// <summary>
    ///     The settings used to configure the mod.
    /// </summary>
    public static TClientSettings Settings = new();

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        IOC.Services.Resolve<IClientNetworkService>()
            .DefaultClientChannel
            .RegisterMessageType<TClientSettings>()
            .SetMessageHandler<TClientSettings>(packet =>
            {
                api.Logger.GantryDebug($"Settings for {GetType().Name}:");
                api.Logger.GantryDebug(packet.ToXml());
                Settings = packet;
            });
    }
}