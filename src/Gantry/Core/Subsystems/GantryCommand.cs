using Gantry.Core.Monads;
using Vintagestory.API.Server;

namespace Gantry.Core.Subsystems;

/// <summary>
///     Represents the Gantry subsystem, providing commands for server control and interaction.
/// </summary>
internal class GantryCommand : UniversalSubsystem
{
    /// <summary>
    ///     Unique identifier for the Gantry subsystem instance.
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();

    /// <summary>
    ///     Indicates whether the subsystem is enabled.
    /// </summary>
    public override bool Enabled => true;

    /// <summary>
    ///     Specifies the execution order for the subsystem, ensuring it executes first by using
    ///     the lowest possible priority.
    /// </summary>
    /// <returns>
    ///     A value of <see cref="double.NegativeInfinity"/> to indicate the lowest execution priority.
    /// </returns>
    public override double ExecuteOrder() => double.NegativeInfinity;

    /// <summary>
    ///     Retrieves a localised string for the Gantry subsystem.
    /// </summary>
    /// <param name="path">The localisation path for the string.</param>
    /// <param name="args">Optional formatting arguments for the string.</param>
    /// <returns>
    ///     A localised string corresponding to the specified path and arguments.
    /// </returns>
    private static string T(string path, params object[] args)
        => LangEx.FeatureStringG("GantryCommand", path, args);

    /// <summary>
    ///     Initialises the Gantry subsystem, registering the "gantry" chat command with the API.
    /// </summary>
    /// <param name="api">The core API instance used to register the command.</param>
    public override void Start(ICoreAPI api)
    {
        _command = api.ChatCommands
            .GetOrCreate("gantry")
            .WithDescription(T("Description"))
            .RequiresPrivilege(ApiEx.OneOf(Privilege.chat, Privilege.controlserver))
            .ToSided();
    }

    private static Sided<IChatCommand> _command = default!;

    /// <summary>
    ///     Main chat command for both sides. Available after .Start();
    /// </summary>
    public static IChatCommand Command => _command.Current;
}