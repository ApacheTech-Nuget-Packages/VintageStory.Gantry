namespace Gantry.Services.Mediator.Filters;

/// <summary>
///     Ensures that a command will only be processed if it is running on the specified app side.
/// </summary>
public class HandledUniversallyAttribute() : HandledOnAttribute(EnumAppSide.Universal);