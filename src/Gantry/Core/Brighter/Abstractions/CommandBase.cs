using System.Diagnostics;
using System.Text.Json.Serialization;
using ApacheTech.Common.BrighterSlim;

namespace Gantry.Core.Brighter.Abstractions;

/// <summary>
///     Acts as a base class for all Brighter commands.
/// </summary>
public abstract class CommandBase : IRequest
{
    /// <summary>
    ///     The unique identifier for this request.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     Gets or sets the span that this operation lives within.
    /// </summary>
    [JsonIgnore]
    public Activity Span { get; set; }
}