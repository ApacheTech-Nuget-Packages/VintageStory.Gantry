namespace Gantry.Services.Mediator.Filters;

/// <summary>
///     Denotes that the decorated object requires a specific mod to be enabled within the gameworld.
/// </summary>
public sealed class RequiresModAttribute : CommandFilterAttribute
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="RequiresModAttribute"/> class.
    /// </summary>
    /// <param name="modIds">The mod identifiers of the required mods.</param>
    public RequiresModAttribute(params string[] modIds) => ModIds = modIds;

    /// <summary>
    ///     The ModIDs of the required mods.
    /// </summary>
    public string[] ModIds { get; }

    /// <inheritdoc />
    public override PipelineFilterPosition Position => PipelineFilterPosition.Prefix;

    /// <inheritdoc />
    public override Type HandlerType => typeof(RequiresModHandler<>);

    internal class RequiresModHandler<TCommand> : CommandFilterHandlerBase<RequiresModAttribute, TCommand>
        where TCommand : ICommand
    {
        private readonly ICoreGantryAPI _gantry;

        public RequiresModHandler(ICoreGantryAPI gantry) => _gantry = gantry;

        public override async Task PrefixAsync(TCommand command, CancellationToken cancellationToken)
        {
            foreach (var modId in Attribute.ModIds)
            {
                if (!_gantry.Uapi.ModLoader.IsModEnabled(modId))
                {
                    throw new ShortCircuitException(statusCode: ActivityStatusCode.Error, rethrow: false,
                        message: $"The command {typeof(TCommand).FullName} requires the mod '{modId}' to be enabled.");
                }
            }
        }
    }
}