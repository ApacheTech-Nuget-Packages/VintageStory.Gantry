using ApacheTech.Common.BrighterSlim;
using JetBrains.Annotations;

namespace Gantry.Core.Brighter.Hosting;

/// <summary>
/// Options around use of RPC over messaging i.e. command and document response
/// Requires blocking for a response on a queue identified by producer to consumer
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal class UseRpc : IUseRpc
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="UseRpc"/> class.
    /// </summary>
    /// <param name="useRpc">if set to <c>true</c> [use RPC].</param>
    /// <param name="replyQueueSubscriptions">The reply queue subscriptions.</param>
    public UseRpc(bool useRpc, IEnumerable<Subscription> replyQueueSubscriptions)
    {
        RPC = useRpc;
        ReplyQueueSubscriptions = replyQueueSubscriptions;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="IUseRpc" /> is RPC.
    /// </summary>
    /// <value><c>true</c> if RPC; otherwise, <c>false</c>.</value>
    public bool RPC { get; set; }

    /// <summary>
    ///     Gets or sets the reply queue subscriptions.
    /// </summary>
    /// <value>The reply queue subscriptions.</value>
    public IEnumerable<Subscription> ReplyQueueSubscriptions { get; set; }
}