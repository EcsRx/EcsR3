using System;
using EcsR3.Entities;
using SystemsR3.Computeds.Collections;

namespace EcsR3.Collections.Entity
{
    public interface IReadOnlyEntityCollection : IComputedCollection<IEntity>
    {
        /// <summary>
        /// The amount of entities that exist
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Gets the entity from the collection, this will return the IEntity or null
        /// </summary>
        /// <param name="id">The Id of the entity you want to locate</param>
        /// <returns>The entity that has been located or null if one could not be found</returns>
        IEntity Get(int id);
        
        /// <summary>
        /// Checks if the collection contains a given entity
        /// </summary>
        /// <param name="id">The Id of the entity you want to locate</param>
        /// <returns>true if it finds the entity, false if it cannot</returns>
        bool Contains(int id);
    }
}