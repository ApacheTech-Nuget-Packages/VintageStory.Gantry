using ApacheTech.Common.BrighterSlim;

// ReSharper disable InconsistentNaming

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Flags support is required for RPC over messaging
///     For RPC a command object needs to return a value on a private queue
///     This approach blocks waiting for a response
/// </summary>
internal interface IUseRpc
{
    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="IUseRpc"/> is RPC.
    /// </summary>
    /// <value><c>true</c> if RPC; otherwise, <c>false</c>.</value>
    bool RPC { get; set; }

    /// <summary>
    ///     Gets or sets the reply queue subscriptions.
    /// </summary>
    /// <value>The reply queue subscriptions.</value>
    IEnumerable<Subscription> ReplyQueueSubscriptions { get; set; }
}