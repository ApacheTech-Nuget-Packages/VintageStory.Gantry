using Gantry.Core.Abstractions.ModSystems;
using Gantry.Core.Network.Extensions;
using Gantry.GameContent.GUI;
using Gantry.GameContent.GUI.Models;
using Gantry.Services.IO.Configuration.Abstractions;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Acts as a base class for EasyX mod systems that require settings from the server.
/// </summary>
public abstract class EasyXClientSystemBase<TModSystem, TClientSettings, TServerSettings> : ClientModSystem<TModSystem>
    where TModSystem : EasyXClientSystemBase<TModSystem, TClientSettings, TServerSettings>
    where TClientSettings : class, IEasyXClientSettings, new()
    where TServerSettings : FeatureSettings<TServerSettings>, IEasyXServerSettings, new()
{
    /// <summary>
    ///     
    /// </summary>
    protected IClientNetworkChannel? ClientChannel { get; private set; }

    /// <summary>
    ///     The settings used to configure the server.
    /// </summary>
    protected TServerSettings ServerSettings { get; private set; } = new();

    /// <summary>
    ///     The settings used to configure the mod.
    /// </summary>
    public TClientSettings Settings { get; private set; } = new();

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        ClientChannel = api.Network
            .GetOrRegisterDefaultChannel(Core)
            .RegisterPacket<SettingsSavedPacket>(Core, ShowSettingsSavedMessageBox)
            .RegisterPacket<TServerSettings>(Core, packet => ServerSettings = packet)
            .RegisterPacket<TClientSettings>(Core, packet =>
            {
                Settings = packet;
                OnSettingsChanged();
            });
    }

    private void ShowSettingsSavedMessageBox(SettingsSavedPacket packet)
    {
        var featureName = packet.FeatureName.StartsWith("Easy")
            ? Core.Lang.Translate(packet.FeatureName, "ModMenu.TabName")
            : Core.Lang.Translate("ModMenu", $"{packet.FeatureName}.TabName");
        var message = Core.Lang.Translate("ModMenu", "SettingsSaved.Message", featureName);
        var title = Core.Lang.Translate("ModMenu", "Title");
        MessageBox.Show(Core, title, message, ButtonLayout.Ok);
    }

    /// <summary>
    ///     Requests the current settings from the server.
    /// </summary>
    public void RequestSettingsFromServer()
    {
        ClientChannel?.SendPacket<TServerSettings>();
    }

    /// <summary>
    ///     An action that fires whenever new settings are received from the server.
    /// </summary>
    protected virtual void OnSettingsChanged() { }
}