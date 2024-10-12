using ApacheTech.Common.BrighterSlim;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core.Brighter.Filters;

/// <summary>
///     Ensures that a command will only be processed if it is running on the specified app side.
/// </summary>
/// <seealso cref="RequestHandlerAttribute" />
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public class HandledUniversallyAttribute(bool asynchronous = false) : HandledOnAttribute(EnumAppSide.Universal, asynchronous);
