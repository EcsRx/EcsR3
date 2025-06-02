using System.Collections.Generic;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using R3;

namespace EcsR3.Systems.Reactive
{
    /// <summary>
    /// This all purpose system just gives you all the entities in a group to
    /// do what you want with
    /// </summary>
    /// <remarks>
    /// This is a half way house between a fully batched system and a ReactToGroupSystem
    /// </remarks>
    public interface IReactToGroupBatchedSystem : IGroupSystem
    {
        /// <summary>
        /// Dictates when the group should be processed
        /// </summary>
        /// <param name="observableGroup">The observable group to process</param>
        /// <returns>The observable chain containing the group</returns>
        /// <remarks>
        /// In most use cases you probably want to run this every update/interval
        /// </remarks>
        Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup);
        
        /// <summary>
        /// The processor for the entity
        /// </summary>
        /// <param name="entityIds">The ids of all the entities in the group</param>
        void Process(IEntityComponentAccessor entityComponentAccessor, IReadOnlyList<Entity> entities);
    }
}