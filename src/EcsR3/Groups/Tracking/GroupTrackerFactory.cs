using System;
using System.Collections.Generic;
using EcsR3.Collections.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using EcsR3.Extensions;
using EcsR3.Groups.Tracking.Trackers;

namespace EcsR3.Groups.Tracking
{
    public class GroupTrackerFactory : IGroupTrackerFactory
    {
        public IEntityChangeRouter EntityChangeRouter { get; }
        public IEntityAllocationDatabase EntityAllocationDatabase { get; }

        public GroupTrackerFactory(IEntityChangeRouter entityChangeRouter, IEntityAllocationDatabase entityAllocationDatabase)
        {
            EntityChangeRouter = entityChangeRouter;
            EntityAllocationDatabase = entityAllocationDatabase;
        }

        public int MatchingComponentCount(Entity entity, int[] componentTypeIds)
        {
            var count = 0;
            for(var i=0;i<componentTypeIds.Length;i++)
            {
                if(EntityAllocationDatabase.HasComponent(componentTypeIds[i], entity))
                { count++; }
            }
            return count;
        }
        
        public IComputedEntityGroupTracker TrackGroup(LookupGroup group, IEnumerable<Entity> initialEntities = null)
        {
            var entityRouterComputedentityGroupTracker = new ComputedEntityGroupTracker(EntityChangeRouter, group);

            if(initialEntities is null)
            { return entityRouterComputedentityGroupTracker; }

            foreach (var entity in initialEntities)
            {
                var matchingRequiredComponents = MatchingComponentCount(entity, group.RequiredComponents);
                var matchingExcludedComponents = MatchingComponentCount(entity, group.ExcludedComponents);
                if(matchingExcludedComponents == 0 && matchingRequiredComponents == 0) { continue; }
                
                var requiredComponentsNeeded = group.RequiredComponents.Length - matchingRequiredComponents;
                var state = new GroupMatchingState(requiredComponentsNeeded, matchingExcludedComponents);
                entityRouterComputedentityGroupTracker.StartTracking(entity, state);
            }
            
            return entityRouterComputedentityGroupTracker;
        }
    }
}