using Gantry.Extensions.Api;
using Gantry.GameContent.Extensions;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Gantry.Extensions;

/// <summary>
///     Extension methods for player location and movement-related operations.
/// </summary>
public static class PlayerLocationExtensions
{
    /// <summary>
    ///     Teleports the player to the supplied target position if it is not <c>null</c>.
    /// </summary>
    /// <param name="api">The core API instance, must be a client API.</param>
    /// <param name="targetPos">The destination position to teleport to, or <c>null</c> to do nothing.</param>
    public static void TeleportTo(this ICoreAPI api, EntityPos? targetPos)
    {
        if (api is not ICoreClientAPI capi) return;
        if (targetPos is null) return;
        capi.AsClientMain().TeleportToPoint(targetPos);
    }

    /// <summary>
    ///     Rotates the player to directly face the supplied position, and teleports them
    ///     to the adjusted position so the camera/player faces the target.
    /// </summary>
    /// <param name="entityPlayer">The player entity to rotate and teleport.</param>
    /// <param name="targetPos">The world co-ordinates to face.</param>
    public static void FacePosition(this EntityPlayer entityPlayer, Vec3d targetPos)
    {
        var entityPos = entityPlayer.Pos.Copy();
        var pos = entityPos.DirectlyFace(targetPos);
        entityPlayer.Api.TeleportTo(pos);
    }
}
