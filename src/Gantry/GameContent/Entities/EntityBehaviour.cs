using Gantry.Extensions.Api;
using Vintagestory.API.Common.Entities;
using static OpenTK.Graphics.OpenGL.GL;

namespace Gantry.GameContent.Entities;

/// <summary>
///     Basic class for entities - a data structures to hold custom
///     information for entities, e.g. for players to hold their stats.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <seealso cref="EntityBehavior" />
public abstract class EntityBehaviour<TEntity> : EntityBehavior where TEntity : Entity
{
    /// <summary>
    ///     The <see cref="Entity"/> this behaviour is applied to.
    /// </summary>
    protected TEntity Entity { get; }

    /// <summary>
    ///     The Gantry API instance, providing access to various game systems and utilities.
    /// </summary>
    protected ICoreGantryAPI Gantry => Entity.Api.GantryCore();

    /// <summary>
    ///     Initialises a new instance of the <see cref="EntityBehaviour{TEntity}"/> class.
    /// </summary>
    /// <param name="entity">The <see cref="Entity"/> this behaviour is applied to.</param>
    /// <exception cref="InvalidCastException">This behaviour cannot be applied to the specified entity.</exception>
    protected EntityBehaviour(Entity entity) : base(entity)
    {
        if (entity is not TEntity e)
        {
            throw new InvalidCastException("This behaviour cannot be applied to the specified entity.");
        }
        Entity = e;
    }
}