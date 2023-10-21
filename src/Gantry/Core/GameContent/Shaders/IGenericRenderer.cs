using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace Gantry.Core.GameContent.Shaders;

/// <summary>
///     Represents a renderer, used to render something on to the client's screen.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IGenericRenderer<TShaderProgram> : IRenderer where TShaderProgram : IGenericShaderProgram
{
    /// <summary>
    ///     The <see cref="IGenericShaderProgram"/> instance to use to render graphics to the screen.
    /// </summary>
    TShaderProgram Shader { get; set; }
}