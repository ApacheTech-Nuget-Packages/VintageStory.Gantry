using Vintagestory.API.Datastructures;

namespace Gantry.GameContent.Behaviours;

/// <summary>
///     Behaviour that prevents a player from dying by restoring a small amount of health
///     and granting temporary invulnerability. Configure the amounts via JSON properties.
/// </summary>
/// <param name="collectible">The owning collectible object.</param>
/// <seealso cref="CollectibleBehavior" />
public class ItemBehaviourPreventPlayerDeath(CollectibleObject collectible) 
    : CollectibleBehavior(collectible)
{
    /// <summary>
    ///     The amount of health restored to the player when this behaviour triggers.
    ///     The value is initialised from the behaviour's JSON properties and defaults to 1.
    /// </summary>
    public float HealthRecovered { get; private set; } = 1f;

    /// <summary>
    ///     The duration, in seconds, of the temporary invulnerability (god mode) granted
    ///     to the player after revival. This is initialised from JSON and defaults to 5 seconds.
    /// </summary>
    public float GodModeCountdown { get; private set; } = 5f;

    /// <summary>
    ///     Initialises the behaviour from the provided JSON properties.
    ///     Reads the <c>healthRecovered</c> and <c>godModeCountdown</c> values, applying defaults
    ///     if they are not present.
    /// </summary>
    /// <param name="properties">The JSON object containing configuration for the behaviour.</param>
    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);
        HealthRecovered = properties["healthRecovered"].AsFloat(1f);
        GodModeCountdown = properties["godModeCountdown"].AsFloat(5f);
    }
}