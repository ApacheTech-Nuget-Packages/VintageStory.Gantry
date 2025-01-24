using Gantry.Core.GameContent.AssetEnum;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Extensions;

/// <summary>
///     Extension methods to aid working with the Render API.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class RenderApiExtensions
{
    /// <summary>
    ///     Discards all currently compiled shaders and re-compiles them.
    /// </summary>
    public static void ReloadShadersThreadSafe(this IShaderAPI api, ICoreClientAPI capi)
    {
        capi.Event.EnqueueMainThreadTask(() => api.ReloadShaders(), "");
    }

    /// <summary>
    ///     Discards all currently compiled shaders and re-compiles them.
    /// </summary>
    public static void ReloadShadersThreadSafe(this IShaderAPI api)
    {
        api.ReloadShadersThreadSafe(ApiEx.Client);
    }

    /// <summary>
    ///     Creates a 2D line mesh between two waypoints.
    /// </summary>
    /// <param name="api">The IRenderAPI instance used to upload the mesh.</param>
    /// <param name="startPoint">The starting waypoint of the line.</param>
    /// <param name="endPoint">The ending waypoint of the line.</param>
    /// <returns>
    ///     A MeshRef object representing the uploaded 2D line mesh.
    /// </returns>
    public static MeshRef Get2dLineMesh(this IRenderAPI api, Waypoint startPoint, Waypoint endPoint)
    {
        var rgbaStart = ColorUtil
            .Int2Hex(startPoint.Color)
            .ToColour()
            .ToNormalisedRgba()
            .ToRgbaVec4F();

        var rgbaEnd = ColorUtil
            .Int2Hex(endPoint.Color)
            .ToColour()
            .ToNormalisedRgba()
            .ToRgbaVec4F();

        var mapManager = IOC.Services.Resolve<WorldMapManager>();

        // Coordinate Translation.
        var startViewPos = Vec2f.Zero;
        var endViewPos = Vec2f.Zero;
        mapManager.TranslateWorldPosToViewPos(startPoint.Position, ref startViewPos);
        mapManager.TranslateWorldPosToViewPos(endPoint.Position, ref endViewPos);

        var mesh = new MeshData();
        mesh.SetXyz([startViewPos.X, startViewPos.Y, 0, endViewPos.X, endViewPos.Y, 0]);
        mesh.SetVerticesCount(2);
        mesh.SetIndices([0, 1]);
        mesh.SetIndicesCount(2);
        mesh.SetRgba(
        [
            (byte)rgbaStart[0], (byte)rgbaStart[1], (byte)rgbaStart[2], (byte)rgbaStart[3],
            (byte)rgbaEnd[0], (byte)rgbaEnd[1], (byte)rgbaEnd[2], (byte)rgbaEnd[3]
        ]);
        mesh.SetMode(EnumDrawMode.Lines);
        return api.UploadMesh(mesh);
    }

    /// <summary>
    ///     Creates a 2D line mesh between two positions with a specified colour.
    /// </summary>
    /// <param name="api">The IRenderAPI instance used to upload the mesh.</param>
    /// <param name="startPos">The starting position of the line.</param>
    /// <param name="endPos">The ending position of the line.</param>
    /// <param name="colour">The colour of the line.</param>
    /// <returns>
    ///     A MeshRef object representing the uploaded 2D line mesh.
    /// </returns>
    public static MeshRef Get2dLineMesh(this IRenderAPI api, Vec2f startPos, Vec2f endPos, NamedColour colour)
    {
        var rgba = colour.ToString()!.ToColour().ToNormalisedRgba();
        var mesh = new MeshData();
        mesh.SetXyz([startPos.X, startPos.Y, 0, endPos.X, endPos.Y, 0]);
        mesh.SetVerticesCount(2);
        mesh.SetIndices([0, 1]);
        mesh.SetIndicesCount(2);
        mesh.SetRgba(
        [
            (byte)rgba[0], (byte)rgba[1], (byte)rgba[2], (byte)rgba[3],
            (byte)rgba[0], (byte)rgba[1], (byte)rgba[2], (byte)rgba[3]
        ]);
        mesh.SetMode(EnumDrawMode.Lines);
        return api.UploadMesh(mesh);
    }

    /// <summary>
    ///     Renders a 2D line using the specified mesh reference.
    /// </summary>
    /// <param name="api">The IRenderAPI instance used to render the mesh.</param>
    /// <param name="mesh">The MeshRef object representing the 2D line mesh.</param>
    public static void Render2DLine(this IRenderAPI api, MeshRef mesh)
    {
        var clientMain = ApiEx.ClientMain;
        var guiShaderProg = ShaderPrograms.Gui;

        // Shader Uniforms.
        guiShaderProg.ExtraGlow = 0;
        guiShaderProg.ApplyColor = 1;
        guiShaderProg.NoTexture = 1f;
        guiShaderProg.OverlayOpacity = 0f;
        guiShaderProg.NormalShaded = 0;

        // Render Line.
        clientMain.GlPushMatrix();
        guiShaderProg.ProjectionMatrix = clientMain.CurrentProjectionMatrix;
        guiShaderProg.ModelViewMatrix = clientMain.CurrentModelViewMatrix;
        api.RenderMesh(mesh);
        clientMain.GlPopMatrix();
    }
}