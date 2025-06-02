using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using R3;

namespace EcsR3.Systems.Reactive
{
    /// <summary>
    /// React To Group systems are like React To Entity systems, but
    /// they observe at the group level not the entity level
    /// </summary>
    /// <remarks>
    /// This is the most common use case system, as it aligns with common
    /// ECS paradigms, i.e a system which triggers every update and runs
    /// on all applicable entities. If you need more control over individual
    /// entity reactions etc then look at ReactToEntity/Data systems.
    /// </remarks>
    public interface IReactToGroupSystem : IGroupSystem
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
        /// <param name="entity">The entity to process</param>
        void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity);
    }
}