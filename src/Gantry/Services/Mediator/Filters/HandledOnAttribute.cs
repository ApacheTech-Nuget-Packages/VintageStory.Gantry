namespace Gantry.Services.Mediator.Filters;

/// <summary>
///     Ensures that a command will only be processed if it is running on the specified app side.
/// </summary>
/// <seealso cref="HandledOnAttribute" />
public class HandledOnAttribute : CommandFilterAttribute
{
    /// <summary>
    ///     Gets the side that this command should work on.
    /// </summary>
    public EnumAppSide Side { get; }

    /// <inheritdoc />
    public override PipelineFilterPosition Position => PipelineFilterPosition.Prefix;

    /// <inheritdoc />
    public override Type HandlerType => typeof(SideHandler<>);

    /// <summary>
    ///  	Initialises a new instance of the <see cref="HandledOnAttribute"/> class.
    /// </summary>
    /// <param name="side">The app side that this command should work on.</param>
    public HandledOnAttribute(EnumAppSide side) => Side = side;

    internal class SideHandler<TCommand> : CommandFilterHandlerBase<HandledOnAttribute, TCommand> where TCommand : ICommand
    {
        private readonly ICoreGantryAPI _gantry;

        public SideHandler(ICoreGantryAPI gantry) => _gantry = gantry;

        public override Task PrefixAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (Attribute.Side is EnumAppSide.Universal || Attribute.Side == _gantry.Side) return Task.CompletedTask;
            throw new ShortCircuitException(
                message: $"This command can only be executed on the {Attribute.Side} side.",
                statusCode: ActivityStatusCode.Error,
                rethrow: true);
        }
    }
}