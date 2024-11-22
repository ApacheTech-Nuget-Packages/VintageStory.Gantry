using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.Hosting;
using Gantry.Core.Hosting.Registration;
using Gantry.Services.MefLab.Abstractions;
using Gantry.Services.Network;
using Vintagestory.API.Server;

namespace Gantry.Services.MefLab;

/// <summary>
///     Provides methods for resolving dependencies, through the Managed Extensibility Framework (MEF).
/// </summary>
public class MefLabServer : MefLabSystem, IMefLabServer, IServerServiceRegistrar
{
    private ICoreServerAPI _sapi;
    private string _channelName;
    private IServerNetworkService _networkService;

    /// <inheritdoc />
    public void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        _sapi = sapi;
        services.AddSingleton<IMefLabServer>(sp =>
        {
            _channelName = IMefLabSystem.ChannelName;
            _networkService = IOC.Services.Resolve<IServerNetworkService>();
            _networkService.RegisterServerChannel(_channelName);
            _networkService.ServerChannel(_channelName)
                .RegisterMessageType<CompositionDataPacket>()
                .SetMessageHandler<CompositionDataPacket>(OnIncomingServerDataPacket);
            return this;
        });
    }

    /// <inheritdoc />
    private void OnIncomingServerDataPacket(IServerPlayer player, CompositionDataPacket packet)
    {
        ResolveContract(packet, player, _sapi);
    }

    /// <inheritdoc />
    public void SendContractToClient(string contractName, FileInfo contractFile)
    {
        _networkService.ServerChannel(_channelName).SendPacket(new CompositionDataPacket
        {
            Contract = contractName,
            Data = File.ReadAllBytes(contractFile.FullName)
        });
    }
}
