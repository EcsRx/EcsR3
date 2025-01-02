﻿using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections.Database;
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

        public IEntityDatabase EntityDatabase { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        public ObservableGroupManager(IObservableGroupFactory observableGroupFactory, IEntityDatabase entityDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ObservableGroupFactory = observableGroupFactory;
            EntityDatabase = entityDatabase;
            ComponentTypeLookup = componentTypeLookup;

            _observableGroups = new ObservableGroupLookup();
        }

        public IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds)
        {
            for (var i = _observableGroups.Count - 1; i >= 0; i--)
            {
                if (_observableGroups[i].Token.LookupGroup.Matches(componentTypeIds))
                { yield return _observableGroups[i]; }
            }
        }

        public IObservableGroup GetObservableGroup(IGroup group, params int[] collectionIds)
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(group);
            
            var observableGroupToken = new ObservableGroupToken(lookupGroup, collectionIds);
            if (_observableGroups.Contains(observableGroupToken)) 
            { return _observableGroups[observableGroupToken]; }

            var configuration = new ObservableGroupConfiguration
            {
                ObservableGroupToken = observableGroupToken
            };

            if (collectionIds != null && collectionIds.Length > 0)
            {
                var targetedCollections = EntityDatabase.Collections.Where(x => collectionIds.Contains(x.Id));
                configuration.NotifyingCollections = targetedCollections;
                configuration.InitialEntities = targetedCollections.GetAllEntities();
            }
            else
            {
                configuration.NotifyingCollections = new[] { EntityDatabase };
                configuration.InitialEntities = EntityDatabase.Collections.GetAllEntities();
            }
            
            var observableGroup = ObservableGroupFactory.Create(configuration);
            _observableGroups.Add(observableGroup);

            return observableGroup;
        }

        public void Dispose()
        {
            foreach (var observableGroup in _observableGroups)
            { (observableGroup as IDisposable)?.Dispose(); }

            EntityDatabase.Dispose();
        }
    }
}