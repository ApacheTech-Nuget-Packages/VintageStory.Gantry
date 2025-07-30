namespace Gantry.GameContent.Extensions;

/// <summary>
///     Extensions methods to aid working with temporal storms.
/// </summary>
public static class TemporalStormServerExtensions
{
    /// <summary>
    ///     Immediately starts a temporal storm.
    /// </summary>
    /// <param name="world">The world to start the storm in.</param>
    /// <param name="stormType">The <see cref="EnumTempStormStrength"/> type of the storm.</param>
    /// <param name="strength">The strength of the storm.</param>
    public static void StartTemporalStorm(this IServerWorldAccessor world, EnumTempStormStrength? stormType = null, double? strength = null)
    {
        var system = world.Api.ModLoader.GetModSystem<SystemTemporalStability>();
        system.StormData.nextStormStrength = stormType ?? system.StormData.nextStormStrength;
        system.StormData.nextStormStrDouble = strength ?? system.StormData.nextStormStrDouble;
        system.StormData.nextStormTotalDays = world.Calendar.TotalDays;
    }

    /// <summary>
    ///     Immediately stops a temporal storm.
    /// </summary>
    /// <param name="world">The world to start the storm in.</param>
    public static void StopTemporalStorm(this IServerWorldAccessor world)
    {
        var system = world.Api.ModLoader.GetModSystem<SystemTemporalStability>();
        if (!system.StormData.nowStormActive) return;
        system.StormData.stormActiveTotalDays = 0;
    }
}