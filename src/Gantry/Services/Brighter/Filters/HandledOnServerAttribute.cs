using ApacheTech.Common.BrighterSlim;

namespace Gantry.Services.Brighter.Filters;

/// <summary>
///     Ensures that a command will only be processed if it is running on the specified app side.
/// </summary>
/// <seealso cref="RequestHandlerAttribute" />
public class HandledOnServerAttribute(bool asynchronous = false) : HandledOnAttribute(EnumAppSide.Server, asynchronous);
