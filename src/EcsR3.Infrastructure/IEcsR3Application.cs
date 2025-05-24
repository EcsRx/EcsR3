using SystemsR3.Infrastructure;
using EcsR3.Collections;
using EcsR3.Collections.Entity;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities.Registries;

namespace EcsR3.Infrastructure
{
    public interface IEcsR3Application : ISystemsR3Application
    {
        /// <summary>
        /// The entity collection, allows you to create and manage entities
        /// </summary>
        IEntityCollection EntityCollection { get; }
        
        /// <summary>
        /// The observable group manager, allows you to get observable groups
        /// </summary>
        IComputedEntityGroupRegistry ComputedEntityGroupRegistry { get; }
    }
}