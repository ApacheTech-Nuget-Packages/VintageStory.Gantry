using ApacheTech.Common.Extensions.Harmony;
using Vintagestory.API.Common.Entities;

namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Extensions methods for the <see cref="ClientMain"/> class.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ClientMainExtensions
{
    /// <summary>
    ///     Stops all currently playing sounds.
    /// </summary>
    public static void StopAllSounds(this ClientMain game)
    {
        var activeSounds = game.GetField<Queue<ILoadedSound>>("ActiveSounds");
        foreach (var sound in activeSounds)
        {
            sound.Stop();
        }
    }

    /// <summary>
    ///     Teleport to a specific point, in a specific heading.
    /// </summary>
    public static void TeleportToPoint(this ClientMain game, EntityPos pos)
    {
        CameraPoint cameraPoint = CameraPoint.FromEntityPos(pos);
        var yaw = cameraPoint.GetField<float>("yaw");
        var pitch = cameraPoint.GetField<float>("pitch");

        game.EntityPlayer.SidedPos.X = cameraPoint.GetField<double>("x");
        game.EntityPlayer.SidedPos.Y = cameraPoint.GetField<double>("y");
        game.EntityPlayer.SidedPos.Z = cameraPoint.GetField<double>("z");
        game.EntityPlayer.SidedPos.Yaw = yaw;
        game.EntityPlayer.SidedPos.Pitch = pitch;
        game.EntityPlayer.SidedPos.Roll = cameraPoint.GetField<float>("roll");
        game.mouseYaw = yaw;
        game.mousePitch = pitch;
    }
}