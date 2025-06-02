using EcsR3.Entities;
using SystemsR3.Computeds;

namespace EcsR3.Collections.Entities
{
    public interface IReadOnlyEntityCollection : IComputedCollection<Entity>
    {
        /// <summary>
        /// Checks if the collection contains a given entity
        /// </summary>
        /// <param name="id">The Id of the entity you want to locate</param>
        /// <returns>true if it finds the entity, false if it cannot</returns>
        bool Contains(Entity entity);
    }
}