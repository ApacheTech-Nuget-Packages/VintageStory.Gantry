namespace Gantry.GameContent.Behaviours;

/// <summary>
///     Behaviour that causes the owning item to be consumed when the entity carrying it dies.
///     Attach this behaviour to an item to ensure it is removed from inventory upon the owner's death.
/// </summary>
/// <param name="collectible">The owning collectible object.</param>
/// <seealso cref="CollectibleBehavior" />
public class ItemBehaviourConsumeOnDeath(CollectibleObject collectible) 
    : CollectibleBehavior(collectible);