using ApacheTech.Common.Extensions.Harmony;
using Vintagestory.API.MathTools;

namespace Gantry.GameContent.Extensions;

/// <summary>
///     Extension Methods for the World Map Manager.
/// </summary>
public static class WorldMapManagerExtensions
{
    /// <summary>
    ///     Returns the map layer used for rendering waypoints.
    /// </summary>
    /// <param name="mapManager">The <see cref="WorldMapManager" /> instance that this method was called from.</param>
    public static WaypointMapLayer? WaypointMapLayer(this WorldMapManager mapManager)
    {
        var layers = mapManager.MapLayers;
        return layers.OfType<WaypointMapLayer>().FirstOrDefault();
    }

    /// <summary>
    ///     Returns the specified map layer.
    /// </summary>
    public static TMapLayer? GetMapLayer<TMapLayer>(this ICoreClientAPI capi) 
        => capi.ModLoader.GetModSystem<WorldMapManager>().MapLayers.OfType<TMapLayer>().FirstOrDefault();

    /// <summary>
    ///     Returns the map layer used for rendering player pins.
    /// </summary>
    /// <param name="mapManager">The <see cref="WorldMapManager" /> instance that this method was called from.</param>
    public static PlayerMapLayer PlayerMapLayer(this WorldMapManager mapManager)
    {
        return mapManager.MapLayers.OfType<PlayerMapLayer>().First();
    }

    /// <summary>
    ///     Trick the server into sending waypoints to the client even if they don't have their map opened. <br/>
    ///     Credit to Novocain.
    /// </summary>
    public static void ForceSendWaypoints(this WorldMapManager worldMapManager)
    {
        var capi = worldMapManager.GetField<ICoreClientAPI>("capi")!;
        capi.Event.EnqueueMainThreadTask(() =>
            capi.Event.RegisterCallback(_ =>
                capi.Network.GetChannel("worldmap")
                    .SendPacket(new OnViewChangedPacket()), 500), "");
    }



    /// <summary>
    ///     Re-centres the map on a specific position.
    /// </summary>
    /// <param name="mapManager">The <see cref="WorldMapManager" /> instance that this method was called from.</param>
    /// <param name="pos">The position to re-centre the map on.</param>
    public static void RecentreMap(this WorldMapManager mapManager, Vec3d pos)
    {
        var map = mapManager.worldMapDlg;
        if (map is null) return;
        UpdateMapGui(map.GetField<GuiComposer>("fullDialog")!, pos);
        UpdateMapGui(map.GetField<GuiComposer>("hudDialog")!, pos);
    }

    private static void UpdateMapGui(GuiComposer composer, Vec3d pos)
    {
        var map = (GuiElementMap)composer.GetElement("mapElem");
        map.CenterMapTo(pos.AsBlockPos);
    }
}