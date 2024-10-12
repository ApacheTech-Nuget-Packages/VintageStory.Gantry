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

    /// <summary>
    ///     Determines whether the execution of the command was successful, or not.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    ///     If the command was not executed successfully, this list may give reasons as to why.
    /// </summary>
    public List<string> ErrorMessages { get; set; } = [];
}