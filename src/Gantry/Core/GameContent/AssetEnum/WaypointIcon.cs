using Gantry.Services.ExtendedEnums;

#pragma warning disable CS1591

namespace Gantry.Core.GameContent.AssetEnum;

/// <summary>
///     The icons that can be used to add waypoints to the map.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[DoNotPruneType]
public sealed class WaypointIcon : StringEnum<WaypointIcon>
{
    public static WaypointIcon Circle { get; } = Create("circle")!;
    public static WaypointIcon Bee { get; } = Create("bee")!;
    public static WaypointIcon Cave { get; } = Create("cave")!;
    public static WaypointIcon Home { get; } = Create("home")!;
    public static WaypointIcon Ladder { get; } = Create("ladder")!;
    public static WaypointIcon Pick { get; } = Create("pick")!;
    public static WaypointIcon Rocks { get; } = Create("rocks")!;
    public static WaypointIcon Ruins { get; } = Create("ruins")!;
    public static WaypointIcon Spiral { get; } = Create("spiral")!;
    public static WaypointIcon Star1 { get; } = Create("star1")!;
    public static WaypointIcon Star2 { get; } = Create("star2")!;
    public static WaypointIcon Trader { get; } = Create("trader")!;
    public static WaypointIcon Vessel { get; } = Create("vessel")!;
}