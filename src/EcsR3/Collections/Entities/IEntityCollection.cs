using System.Collections.Generic;
using EcsR3.Entities;

namespace EcsR3.Collections.Entities
{
    /// <summary>
    /// The entity collection is a container for entities, it can be seen as a Repository of sorts
    /// as it allows for CRUD based operations and querying (through extensions)
    /// </summary>
    public interface IEntityCollection : IReadOnlyEntityCollection
    {
        /// <summary>
        /// This will create and return a new entity.
        /// If required you can pass in a blueprint which the created entity will conform to
        /// </summary>
        /// <param name="id">Id to use for the entity (defaults to null, meaning it'll automatically get the next available id)</param>
        /// <returns></returns>
        Entity Create(int? id = null);

        /// <summary>
        /// Creates many entities at once
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Entity[] CreateMany(int count);
        
        /// <summary>
        /// This will remove the entity from the collection and optionally destroy the entity.
        /// It is worth noting if you try to remove an entity id that does not exist you will get an exception
        /// </summary>
        /// <param name="id">The Id of the entity you want to remove</param>
        void Remove(Entity id);
        
        /// <summary>
        /// Removes many entities at once
        /// </summary>
        /// <param name="ids"></param>
        void Remove(IReadOnlyList<Entity> entities);
        
        /// <summary>
        /// Removes all entities from the collection
        /// </summary>
        void Clear();
    }
}
