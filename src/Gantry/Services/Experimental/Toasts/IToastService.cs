using System.Drawing;

namespace Gantry.Services.Experimental.Toasts;

/// <summary>
///     Provides a service for displaying toast notifications in the client UI.
/// </summary>
internal interface IToastService
{
    /// <summary>
    ///     The name of the network channel used by the toast service.
    /// </summary>
    public const string ChannelName = "GantryToastService";

    /// <summary>
    ///     Displays a toast notification using an asset location.
    /// </summary>
    /// <param name="asset">The asset location to display as a toast.</param>
    void Show(AssetLocation asset);

    /// <summary>
    ///     Displays a toast notification with the specified message and default settings.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="ttl">The time-to-live for the toast, in seconds.</param>
    void Show(string message, float ttl = 2f);

    /// <summary>
    ///     Displays a toast notification with the specified message, colour, and time-to-live.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="colour">The colour of the toast text.</param>
    /// <param name="ttl">The time-to-live for the toast, in seconds.</param>
    void Show(string message, Color colour, float ttl = 2f);

    /// <summary>
    ///     Queues a toast notification to be displayed.
    /// </summary>
    /// <param name="toast">The toast notification to display.</param>
    void Show(Toast toast);
}