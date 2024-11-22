using JetBrains.Annotations;
using OpenTK.Mathematics;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.Extensions;

/// <summary>
///     Extension methods for when working with entities.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class EntityExtensions
{
    /// <summary>
    ///     Applies a force to an entity, for a given direction vector.
    /// </summary>
    /// <param name="entity">The entity to apply the force to.</param>
    /// <param name="forwardVec">The forward direction vector, in which to apply the force.</param>
    /// <param name="force">The amount of force to apply.</param>
    public static void ApplyForce(this Entity entity, Vec3d forwardVec, double force)
    {
        entity.Pos.Motion.X *= forwardVec.X * force;
        entity.Pos.Motion.Y *= forwardVec.Y * force;
        entity.Pos.Motion.Z *= forwardVec.Z * force;
    }

    /// <summary>
    ///     Changes the facing of a given agent, to face directly away from a target gameworld location.
    /// </summary>
    /// <param name="agentPos">The agent's position.</param>
    /// <param name="targetPos">The target position.</param>
    /// <returns>An <see cref="EntityPos"/>, containing the agent's current XYZ position, and the new YPR rotations.</returns>
    public static EntityPos LookAwayFrom(this EntityPos agentPos, Vec3d targetPos)
    {
        var targetDirection = agentPos.XYZ.RelativeRotationalDirection(targetPos);
        var entityPos = agentPos.Copy();
        entityPos.Pitch = targetDirection.X;
        entityPos.Yaw = (targetDirection.Y + GameMath.PI) % GameMath.TWOPI;
        return entityPos;
    }

    /// <summary>
    ///     Changes the facing of a given agent, to face directly towards a target gameworld location.
    /// </summary>
    /// <param name="agentPos">The agent's position.</param>
    /// <param name="targetPos">The target position.</param>
    /// <returns>An <see cref="EntityPos"/>, containing the agent's current XYZ position, and the new YPR rotations.</returns>
    public static EntityPos LookAt(this EntityPos agentPos, Vec3d targetPos)
    {
        var targetDirection = agentPos.XYZ.RelativeRotationalDirection(targetPos);
        var entityPos = agentPos.Copy();
        entityPos.Pitch = GameMath.PI - targetDirection.X;
        entityPos.Yaw = targetDirection.Y % GameMath.TWOPI;
        return entityPos;
    }

    /// <summary>
    ///     Determines the relative rotational direction as a quaternion between two locations.
    /// </summary>
    /// <param name="sourcePos">The source position.</param>
    /// <param name="targetPos">The target position.</param>
    /// <returns>A <see cref="Quaternion"/> containing the rotational values around all three axes, as if the target position was facing directly towards the source.</returns>
    public static Quaternion RelativeRotationalDirectionQuaternion(this Vec3d sourcePos, Vec3d targetPos)
    {
        var direction = targetPos - sourcePos;
        direction.Normalize();
        var yaw = (float)Math.Atan2(direction.Z, direction.X);
        var pitch = (float)Math.Asin(direction.Y);
        return Quaternion.FromEulerAngles(0, yaw, pitch);
    }

    /// <summary>
    ///     Determines the relative rotational direction vector between two locations.
    /// </summary>
    /// <param name="sourcePos">The source position.</param>
    /// <param name="targetPos">The target position.</param>
    /// <returns>A <see cref="Vec2f"/> containing the normalised rotational values around the X, and Y axes, as if the target position was facing directly towards the source.</returns>
    public static Vec2f RelativeRotationalDirection(this Vec3d sourcePos, Vec3d targetPos)
    {
        var cartesianCoordinates = targetPos.SubCopy(sourcePos).Normalize();
        var yaw = GameMath.TWOPI - (float)Math.Atan2(cartesianCoordinates.Z, cartesianCoordinates.X);
        var pitch = (float)Math.Asin(cartesianCoordinates.Y);
        return new Vec2f(pitch, yaw);
    }

    /// <summary>
    ///     Determines whether an agent is looking at the specified target position.
    /// </summary>
    /// <param name="agent">The agent.</param>
    /// <param name="targetPos">The target position.</param>
    /// <param name="radThreshold">The margin of error threshold, in radians.</param>
    /// <returns><c>true</c> if the agent is looking at the specified target position; otherwise, <c>false</c>.</returns>
    public static bool IsLookingAt(this EntityAgent agent, Vec3d targetPos, float radThreshold)
    {
        var agentPos = agent.SidedPos;
        var relativePos = agentPos.Copy().LookAt(targetPos);

        var pitchDiff = Math.Abs(agentPos.Pitch) - Math.Abs(relativePos.Pitch);
        var yawDiff = Math.Abs(agentPos.Yaw) - Math.Abs(relativePos.Yaw);

        var pitchIsWithinThreshold = -radThreshold <= pitchDiff && pitchDiff <= radThreshold;
        var yawIsWithinThreshold = -radThreshold <= yawDiff && yawDiff <= radThreshold;

        return pitchIsWithinThreshold && yawIsWithinThreshold;
    }

    /// <summary>
    ///  Determines whether the agent is looking at the sun, within a specified margin of error.
    /// </summary>
    /// <param name="agent">The agent.</param>
    /// <param name="radThreshold">The margin of error threshold, in radians.</param>
    /// <returns><c>true</c> if the agent is looking at the sun; otherwise, <c>false</c>.</returns>
    public static bool IsLookingAtTheSun(this EntityAgent agent, float radThreshold)
    {
        var api = agent.Api;
        if (api.Side.IsServer()) return false;
        var capi = (ICoreClientAPI)api;
        return agent.IsLookingAt(capi.World.Calendar.SunPosition.ToVec3d(), radThreshold);
    }

    /// <summary>
    ///  Determines whether the agent is looking at the moon, within a specified margin of error.
    /// </summary>
    /// <param name="agent">The agent.</param>
    /// <param name="radThreshold">The margin of error threshold, in radians.</param>
    /// <returns><c>true</c> if the agent is looking at the moon; otherwise, <c>false</c>.</returns>
    public static bool IsLookingAtTheMoon(this EntityAgent agent, float radThreshold)
    {
        var api = agent.Api;
        if (api.Side.IsServer()) return false;
        var capi = (ICoreClientAPI)api;
        return agent.IsLookingAt(capi.World.Calendar.MoonPosition.ToVec3d(), radThreshold);
    }
}