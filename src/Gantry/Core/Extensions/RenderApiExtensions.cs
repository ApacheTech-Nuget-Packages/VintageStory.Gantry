using JetBrains.Annotations;
using Vintagestory.API.Client;

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
}