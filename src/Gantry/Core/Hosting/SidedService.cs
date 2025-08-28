using Gantry.Core.Abstractions;

namespace Gantry.Core.Hosting;

/// <summary>
///     Represents a service that provides access to both client and server instances of a specified type.
/// </summary>
public class SidedService<TClientInstance, TServerInstance>
{
    private readonly ICoreGantryAPI _core;

    /// <summary>
    ///     Initialises a new instance of the <see cref="SidedService{TClientInstance, TServerInstance}" /> class.
    /// </summary>
    public SidedService(ICoreGantryAPI core) => _core = core;

    /// <summary>
    ///     The client instance of the specified type.
    /// </summary>
    public TClientInstance Client => _core.Services.GetRequiredService<TClientInstance>();

    /// <summary>
    ///     The server instance of the specified type.
    /// </summary>
    public TServerInstance Server => _core.Services.GetRequiredService<TServerInstance>();
}
