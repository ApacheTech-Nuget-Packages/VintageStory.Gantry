using Gantry.Services.Network.Extensions;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public abstract class EasyXClientSystemBase<TClientSettings> : ClientModSystem
    where TClientSettings : class, IEasyXClientSettings, new()
{
    /// <summary>
    ///     The settings used to configure the mod.
    /// </summary>
    public static TClientSettings Settings { get; private set; } = new();

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Network
            .GetOrRegisterDefaultChannel()
            .RegisterPacket<TClientSettings>(packet =>
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