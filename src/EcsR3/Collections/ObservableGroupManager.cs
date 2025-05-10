using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Components.Lookups;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Lookups;
using EcsR3.Extensions;

namespace EcsR3.Collections
{
    public class ObservableGroupManager : IObservableGroupManager
    {
        public ObservableGroupLookup _observableGroups { get; }

        public IReadOnlyList<IObservableGroup> ObservableGroups => _observableGroups;

        public IEntityCollection EntityCollection { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        private readonly object _lock = new object();
        
        public ObservableGroupManager(IObservableGroupFactory observableGroupFactory, IEntityCollection entityCollection, IComponentTypeLookup componentTypeLookup)
        {
            ObservableGroupFactory = observableGroupFactory;
            EntityCollection = entityCollection;
            ComponentTypeLookup = componentTypeLookup;

            _observableGroups = new ObservableGroupLookup();
        }

        public IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds)
        {
            lock (_lock)
            {
                for (var i = _observableGroups.Count - 1; i >= 0; i--)
                {
                    if (_observableGroups[i].Group.Matches(componentTypeIds))
                    { yield return _observableGroups[i]; }
                }
            }
        }

        public IObservableGroup GetObservableGroup(IGroup group, params int[] collectionIds)
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(group);
            
            lock (_lock)
            {
                if (_observableGroups.TryGetValue(lookupGroup, out var existingEObservableGroup))
                { return existingEObservableGroup; }
                
                var observableGroup = ObservableGroupFactory.Create(lookupGroup);
                _observableGroups.Add(observableGroup);

                return observableGroup;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var observableGroup in _observableGroups)
                { observableGroup?.Dispose(); }
            }
        }
    }
}