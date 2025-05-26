using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;
using EcsR3.Groups.Tracking.Trackers;

namespace EcsR3.Groups.Tracking
{
    public class GroupTrackerFactory : IGroupTrackerFactory
    {
        public IEntityChangeRouter EntityChangeRouter { get; }

        public GroupTrackerFactory(IEntityChangeRouter entityChangeRouter)
        { EntityChangeRouter = entityChangeRouter; }

        public IComputedEntityGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities = null)
        {
            var observableGroupTracker = new EntityRouterComputedEntityGroupTracker(EntityChangeRouter, group);

            if(initialEntities is null)
            { return observableGroupTracker; }

            foreach (var entity in initialEntities)
            {
                var matchingRequiredComponents = entity.MatchingComponentCount(group.RequiredComponents);
                var matchingExcludedComponents = entity.MatchingComponentCount(group.ExcludedComponents);
                if(matchingExcludedComponents == 0 && matchingRequiredComponents == 0) { continue; }
                
                var requiredComponentsNeeded = group.RequiredComponents.Length - matchingRequiredComponents;
                var state = new GroupMatchingState(requiredComponentsNeeded, matchingExcludedComponents);
                observableGroupTracker.StartTracking(entity.Id, state);
            }
            
            return observableGroupTracker;
        }
    }
}