using Gantry.Core.Abstractions;

namespace Gantry.Core.Hosting;

/// <summary>
///     An interface representing a mod host in the Gantry framework.
/// </summary>
public interface IModHost
{
    /// <inheritdoc />
    internal void InitialiseCore(ICoreAPI api);
}