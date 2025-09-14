using ApacheTech.Common.Extensions.Harmony;
using Vintagestory.API.Common.Entities;

namespace Gantry.Extensions.Api;

/// <summary>
///     Extensions methods for the <see cref="ClientMain"/> class.
/// </summary>
public static class ClientMainExtensions
{
    /// <summary>
    ///     Stops all currently playing sounds.
    /// </summary>
    public static void StopAllSounds(this ClientMain game)
    {
        var activeSounds = game.GetField<Queue<ILoadedSound>>("ActiveSounds")!;
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
        var t = Traverse.Create(CameraPoint.FromEntityPos(pos));
        var yaw = t.Field<float>("yaw").Value;
        var pitch = t.Field<float>("pitch").Value;

        game.EntityPlayer.SidedPos.X = t.Field<double>("x").Value;
        game.EntityPlayer.SidedPos.Y = t.Field<double>("y").Value;
        game.EntityPlayer.SidedPos.Z = t.Field<double>("z").Value;
        game.EntityPlayer.SidedPos.Yaw = yaw;
        game.EntityPlayer.SidedPos.Pitch = pitch;
        game.EntityPlayer.SidedPos.Roll = t.Field<float>("roll").Value;
        game.mouseYaw = yaw;
        game.mousePitch = pitch;
    }
}