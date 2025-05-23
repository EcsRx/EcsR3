using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Entity;
using EcsR3.Components.Lookups;
using EcsR3.Computeds.Groups;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Extensions;
using SystemsR3.Extensions;

namespace EcsR3.Collections
{
    public class ComputedGroupManager : IComputedGroupManager
    {
        public Dictionary<LookupGroup, IComputedEntityGroup> _computedGroups { get; }

        public IEnumerable<IComputedEntityGroup> ComputedGroups => _computedGroups.Values;

        public IEntityCollection EntityCollection { get; }
        public IComputedEntityGroupFactory ComputedEntityGroupFactory { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        private readonly object _lock = new object();
        
        public ComputedGroupManager(IComputedEntityGroupFactory computedEntityGroupFactory, IEntityCollection entityCollection, IComponentTypeLookup componentTypeLookup)
        {
            ComputedEntityGroupFactory = computedEntityGroupFactory;
            EntityCollection = entityCollection;
            ComponentTypeLookup = componentTypeLookup;

            _computedGroups = new Dictionary<LookupGroup, IComputedEntityGroup>();
        }

        public IEnumerable<IComputedEntityGroup> GetApplicableGroups(int[] componentTypeIds)
        {
            lock (_lock)
            { return ComputedGroups.Where(x => x.Group.Matches(componentTypeIds)); }
        }

        public IComputedEntityGroup GetComputedGroup(IGroup group)
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(group);
            
            lock (_lock)
            {
                if (_computedGroups.TryGetValue(lookupGroup, out var existingComputedGroup))
                { return existingComputedGroup; }
                
                var computedGroup = ComputedEntityGroupFactory.Create(lookupGroup);
                _computedGroups.Add(lookupGroup, computedGroup);

                return computedGroup;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            { _computedGroups.Values.DisposeAll(); }
        }
    }
}