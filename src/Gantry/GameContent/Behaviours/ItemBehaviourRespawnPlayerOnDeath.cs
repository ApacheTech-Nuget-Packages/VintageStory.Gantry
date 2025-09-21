namespace Gantry.GameContent.Behaviours;

/// <summary>
///     Behaviour that causes the owning item to respawn the player upon death.
///     Attach this behaviour to an item to ensure the player is respawned when the
///     item's carrier dies.
/// </summary>
/// <param name="collectible">The owning collectible object.</param>
/// <seealso cref="CollectibleBehavior" />
public class ItemBehaviourRespawnPlayerOnDeath(CollectibleObject collectible) 
    : CollectibleBehavior(collectible);