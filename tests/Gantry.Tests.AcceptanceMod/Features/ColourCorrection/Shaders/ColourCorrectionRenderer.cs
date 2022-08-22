using ApacheTech.Common.Extensions.System;
using Gantry.Core.GameContent.Shaders;
using Vintagestory.API.Client;

// ReSharper disable ClassNeverInstantiated.Global

namespace Gantry.Tests.AcceptanceMod.Features.ColourCorrection.Shaders
{
    /// <summary>
    ///     Changes the hue, and saturation of the scene colours, based on user-defined settings.
    /// </summary>
    /// <seealso cref="IGenericRenderer{TShaderProgram}" />
    public sealed class ColourCorrectionRenderer : IGenericRenderer<IGenericShaderProgram>
    {
        private readonly ICoreClientAPI _capi;

        private readonly MeshRef _quadRef;
        private readonly ColourCorrectionSettings _settings;

        /// <summary>
        /// 	Initialises a new instance of the <see cref="ColourCorrectionRenderer"/> class.
        /// </summary>
        /// <param name="capi">The capi.</param>
        /// <param name="settings">The settings.</param>
        public ColourCorrectionRenderer(ICoreClientAPI capi, ColourCorrectionSettings settings)
        {
            _capi = capi;
            _settings = settings;
            _quadRef = _capi.Render.UploadMesh(QuadMeshUtil
                .GetCustomQuadModelData(-1f, -1f, 0f, 2f, 2f)
                .With(p => p.Rgba = null));
        }

        /// <summary>
        ///     Gets or sets the shader used for this renderer. This is a transient dependency that can be disposed from outside of the class, when shaders are reloaded.
        /// </summary>
        /// <value>The shader.</value>
        public IGenericShaderProgram Shader { get; set; }

        /// <inheritdoc />
        public double RenderOrder => 2f;

        /// <inheritdoc />
        public int RenderRange => 0;

        /// <summary>
        ///     Called before each frame is finalised, on screen.
        /// </summary>
        /// <param name="dt">The time in milliseconds, between this frame, and the last successfully rendered frame.</param>
        /// <param name="stage">The current render stage.</param>
        public void OnRenderFrame(float dt, EnumRenderStage stage)
        {
            if (!_settings.Enabled) return;

            var currentActiveShader = _capi.Render.CurrentActiveShader;
            currentActiveShader?.Stop();
            
            if (!Shader.Disposed)
            {
                Shader.Use();
                _capi.Render.GlToggleBlend(true, EnumBlendMode.Overlay);
                _capi.Render.GLDisableDepthTest();
                Shader.UpdateUniforms();
                _capi.Render.RenderMesh(_quadRef);
                Shader.Stop();
            }

            currentActiveShader?.Use();
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            _quadRef?.Dispose();
            Shader?.Dispose();
        }
    }
}