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

        public int MatchingComponentCount(int entityId, int[] componentTypeIds)
        {
            var count = 0;
            for(var i=0;i<componentTypeIds.Length;i++)
            {
                if(EntityAllocationDatabase.HasComponent(componentTypeIds[i], entityId))
                { count++; }
            }
            return count;
        }
        
        public IComputedEntityGroupTracker TrackGroup(LookupGroup group, IEnumerable<int> initialEntityIds = null)
        {
            var entityRouterComputedentityGroupTracker = new ComputedEntityGroupTracker(EntityChangeRouter, group);

            if(initialEntityIds is null)
            { return entityRouterComputedentityGroupTracker; }

            foreach (var entityId in initialEntityIds)
            {
                var matchingRequiredComponents = MatchingComponentCount(entityId, group.RequiredComponents);
                var matchingExcludedComponents = MatchingComponentCount(entityId, group.ExcludedComponents);
                if(matchingExcludedComponents == 0 && matchingRequiredComponents == 0) { continue; }
                
                var requiredComponentsNeeded = group.RequiredComponents.Length - matchingRequiredComponents;
                var state = new GroupMatchingState(requiredComponentsNeeded, matchingExcludedComponents);
                entityRouterComputedentityGroupTracker.StartTracking(entityId, state);
            }
            
            return entityRouterComputedentityGroupTracker;
        }
    }
}