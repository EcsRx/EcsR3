using EcsR3.Entities.Accessors;
using R3;

namespace EcsR3.Systems.Reactive
{
    /// <summary>
    /// React to data systems are a more advanced version of React To Entity systems,
    /// they allow you to not only react to entity changes but also pass back custom
    /// payloads to the executor, use this if you need to react with custom data
    /// </summary>
    /// <typeparam name="T">The type of payload to react with</typeparam>
    public interface IReactToDataSystem<T> : IGroupSystem
    {
        /// <summary>
        /// Returns and observable indicating both when the system should execute for a given entity
        /// as well as what the data should be to be passed to the executor
        /// </summary>
        /// <param name="entity">The entity to react to</param>
        /// <returns>Observable containing data</returns>
        Observable<T> ReactToData(IEntityComponentAccessor entityComponentAccessor, int entityId);

        /// <summary>
        /// The executor which is passed both the entity and the data from the reaction
        /// </summary>
        /// <param name="entity">The entity to use</param>
        /// <param name="reactionData">The data from the reaction</param>
        void Process(IEntityComponentAccessor entityComponentAccessor, int entityId, T reactionData);
    }
}