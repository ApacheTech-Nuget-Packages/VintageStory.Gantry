﻿using ApacheTech.Common.BrighterSlim;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core.Brighter.Filters;

/// <summary>
///     Ensures that a command will only be processed if it is running on the specified app side.
/// </summary>
/// <seealso cref="RequestHandlerAttribute" />
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public class SideAttribute : RequestHandlerAttribute
{
    /// <summary>
    ///     Gets the side that this command should work on.
    /// </summary>
    public EnumAppSide Side { get; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="SideAttribute"/> should be handled asynchronously.
    /// </summary>
    /// <value><c>true</c> if the handler is asynchronous; otherwise, <c>false</c>.</value>
    public bool Asynchronous { get; }

    /// <summary>
    ///  	Initialises a new instance of the <see cref="SideAttribute"/> class.
    /// </summary>
    /// <param name="side">The app side that this command should work on.</param>
    /// <param name="asynchronous">Determines whether this handler should be handled asynchronously.</param>
    public SideAttribute(EnumAppSide side, bool asynchronous = false) : base(1)
    {
        Side = side;
        Asynchronous = asynchronous;
    }

    /// <inheritdoc />
    public override Type GetHandlerType()
    {
        return typeof(SideHandler<>);
    }

    /// <inheritdoc />
    public override object[] InitializerParams() => [Side];
}

/// <summary />
public class SideHandler<TRequest> : RequestHandler<TRequest> where TRequest : class, IRequest
{
    private EnumAppSide _side;
    /// <summary />
    public override void InitializeFromAttributeParams(params object[] initialiserList) => _side = (EnumAppSide)initialiserList[0];
    /// <summary />
    public override TRequest Handle(TRequest command) => _side.IsUniversal() || ApiEx.Side == _side ? base.Handle(command) : base.Fallback(command);
}