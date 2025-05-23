using EcsR3.Collections.Entity;
using EcsR3.Entities;
using EcsR3.Groups;
using R3;
using SystemsR3.Computeds.Collections;

namespace EcsR3.Computeds.Groups
{
    /// <summary>
    /// A maintained collection of entities which match a given group
    /// </summary>
    /// <remarks>
    /// Implements IEnumerable so you can just enumerate the entities within the group.
    /// 
    /// This is quite often going to be a cached list of entity ids which
    /// is kept up to date based off events being reported, so it is often
    /// more performant to use this rather than querying a collection directly.
    /// This can change based upon implementations though.
    /// </remarks>
    public interface IComputedEntityGroup : IReadOnlyEntityCollection
    {
        /// <summary>
        /// The underlying group
        /// </summary>
        /// <remarks>
        /// The token contains both components required
        /// </remarks>
        LookupGroup Group { get; }
        
        /// <summary>
        /// Event stream for when an entity is about to be removed from this group
        /// </summary>
        Observable<IEntity> OnRemoving { get; }
    }
}