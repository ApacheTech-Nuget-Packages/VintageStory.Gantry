using System.Drawing;
using Gantry.Services.Network.Extensions;
using Vintagestory.API.Server;

#pragma warning disable IDE1006 // Naming Styles

namespace Gantry.Services.Toasts;

/// <summary>
///     Provides a service for displaying toast notifications in the client UI.
/// </summary>
public class ToastService : UniversalSubsystem, IToastService
{
    private long _listenerId;
    private const int MaxActive = 5;

    /// <inheritdoc />
    public override void ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi)
    {
        services.AddSingleton<IToastService>(this);
    }

    /// <inheritdoc />
    public override void StartServerSide(ICoreServerAPI sapi)
    {
        // Networking
        sapi.Network
            .RegisterChannel(IToastService.ChannelName)
            .RegisterMessageType<AssetLocation>()
            .RegisterMessageType<Toast>();
    }

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI capi)
    {
        // Networking
        capi.Network
            .RegisterChannel(IToastService.ChannelName)
            .RegisterMessageHandler<AssetLocation>(Show)
            .RegisterMessageHandler<Toast>(Show);

        // Listener
        _listenerId = capi.Event.RegisterGameTickListener(OnGameTick, 30);
    }

    /// <summary>
    ///     Invoked periodically to update and display active toasts.
    /// </summary>
    /// <param name="obj">The time elapsed since the last update, in seconds.</param>
    private void OnGameTick(float obj)
    {
        if (Pending.Count <= 0) return;

        if (Active.Count < MaxActive)
        {
            for (var i = 0; i < MaxActive - Active.Count; i++)
            {
                if (i > Pending.Count) break;
                var toast = Pending.Dequeue();
                Active.Enqueue(new ToastHudElement(Capi, toast));
            }
        }
        else
        {
            Active.First().Kill();
        }
    }

    /// <summary>
    ///     Gets the queue of active toast notifications currently being displayed.
    /// </summary>
    internal Queue<ToastHudElement> Active { get; set; } = new();

    /// <summary>
    ///     Gets the queue of pending toast notifications waiting to be displayed.
    /// </summary>
    internal Queue<Toast> Pending { get; set; } = new();

    /// <inheritdoc />
    public void Show(AssetLocation asset)
    {
        try
        {
            Show(asset.ToShortString());
        }
        catch (Exception ex)
        {
            G.Logger.Error("Toast failed to render.");
            G.Logger.Error(ex);
        }
    }

    /// <inheritdoc />
    public void Show(string message, float ttl = 2f)
        => Show(message, Color.White, ttl);

    /// <inheritdoc />
    public void Show(string message, Color colour, float ttl = 2f)
        => Show(new Toast(message, colour, ttl));

    /// <inheritdoc />
    public void Show(Toast toast)
        => Pending.Enqueue(toast);

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
        Capi.Event.UnregisterGameTickListener(_listenerId);
    }
}