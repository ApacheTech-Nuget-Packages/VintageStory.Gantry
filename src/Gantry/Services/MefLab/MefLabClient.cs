using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.Hosting;
using Gantry.Core.Hosting.Registration;
using Gantry.Services.MefLab.Abstractions;
using Gantry.Services.Network;
using Vintagestory.API.Client;

namespace Gantry.Services.MefLab;

/// <summary>
///     Provides methods for resolving dependencies, through the Managed Extensibility Framework (MEF).
/// </summary>
public abstract class MefLabClient : MefLabSystem, IMefLabClient, IClientServiceRegistrar
{
    private ICoreClientAPI _capi;
    private string _channelName;
    private IClientNetworkService _networkService;

    /// <summary>
    /// 
    /// </summary>
    public void ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi)
    {
        _capi = capi;
        services.AddSingleton<IMefLabClient>(sp =>
        {
            _channelName = IMefLabSystem.ChannelName;
            _networkService = IOC.Services.Resolve<IClientNetworkService>();
            _networkService.RegisterClientChannel(_channelName);
            _networkService.ClientChannel(_channelName)
                .RegisterMessageType<CompositionDataPacket>()
                .SetMessageHandler<CompositionDataPacket>(OnIncomingClientDataPacket);
            return this;
        });
    }

    /// <inheritdoc />
    private void OnIncomingClientDataPacket(CompositionDataPacket packet)
    {
        ResolveContract(packet, _capi.World.Player, _capi);
    }

    /// <inheritdoc />
    public void SendContractToServer(string contractName, FileInfo contractFile)
    {
        _networkService.ClientChannel(_channelName).SendPacket(new CompositionDataPacket
        {
            Contract = contractName,
            Data = File.ReadAllBytes(contractFile.FullName)
        });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
