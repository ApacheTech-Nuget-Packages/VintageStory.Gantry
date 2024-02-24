﻿using JetBrains.Annotations;

namespace Gantry.Core.Brighter.Hosting;

/// <summary>
///     The Transformer Registry is used to help us register with the Service Collection when using Brighter .NET Core Support
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface ITransformerRegistry
{
    /// <summary>
    ///     Register a transform with the IServiceCollection using the ServiceLifetime
    /// </summary>
    /// <param name="transform">The type of the transform to register</param>
    void Add(Type transform);
}