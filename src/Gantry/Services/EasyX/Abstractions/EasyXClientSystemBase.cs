using Gantry.Core.Abstractions;
using Gantry.Core.Abstractions.ModSystems;
using Gantry.Core.Network.Extensions;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Acts as a base class for EasyX mod systems that require settings from the server.
/// </summary>
public abstract class EasyXClientSystemBase<TModSystem, TClientSettings> : ClientModSystem<TModSystem>
    where TModSystem : EasyXClientSystemBase<TModSystem, TClientSettings>
    where TClientSettings : class, IEasyXClientSettings, new()
{
    /// <summary>
    ///     The settings used to configure the mod.
    /// </summary>
    public TClientSettings Settings { get; private set; } = new();

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Network
            .GetOrRegisterDefaultChannel(Core)
            .RegisterPacket<TClientSettings>(Core, packet =>
            {
                Settings = packet;
                OnSettingsChanged();
            });
    }

    /// <summary>
    ///     An action that fires whenever new settings are received from the server.
    /// </summary>
    protected virtual void OnSettingsChanged() { }
}