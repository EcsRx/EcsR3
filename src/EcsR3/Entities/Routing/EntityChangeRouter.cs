using System.Collections.Generic;
using EcsR3.Components.Lookups;
using R3;
using SystemsR3.Extensions;

namespace EcsR3.Entities.Routing
{
    public class EntityChangeRouter : IEntityChangeRouter
    {
        private readonly Dictionary<ComponentContract, Subject<EntityChanges>> _onComponentAddedForGroup = new Dictionary<ComponentContract, Subject<EntityChanges>>();
        private readonly Dictionary<ComponentContract, Subject<EntityChanges>> _onComponentRemovingForGroup = new Dictionary<ComponentContract, Subject<EntityChanges>>();
        private readonly Dictionary<ComponentContract, Subject<EntityChanges>> _onComponentRemovedForGroup = new Dictionary<ComponentContract, Subject<EntityChanges>>();
        
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public EntityChangeRouter(IComponentTypeLookup componentTypeLookup)
        {
            ComponentTypeLookup = componentTypeLookup;
        }

        public void Dispose()
        {
            _onComponentAddedForGroup.ForEachRun(x => x.Value.Dispose());
            _onComponentRemovingForGroup.ForEachRun(x => x.Value.Dispose());
            _onComponentRemovedForGroup.ForEachRun(x => x.Value.Dispose());
        }

        public Observable<EntityChanges> OnEntityAddedComponents(params int[] componentTypes)
        {
            var contract = new ComponentContract(componentTypes);
            if(_onComponentAddedForGroup.TryGetValue(contract, out var existingObservable))
            { return existingObservable; }

            var newSub = new Subject<EntityChanges>();
            _onComponentAddedForGroup.Add(contract, newSub);
            return newSub;
        }
        
        public Observable<EntityChanges> OnEntityRemovingComponents(params int[] componentTypes)
        {
            var contract = new ComponentContract(componentTypes);
            if(_onComponentRemovingForGroup.TryGetValue(contract, out var existingObservable))
            { return existingObservable; }

            var newSub = new Subject<EntityChanges>();
            _onComponentRemovingForGroup.Add(contract, newSub);
            return newSub;
        }
        
        public Observable<EntityChanges> OnEntityRemovedComponents(params int[] componentTypes)
        {
            var contract = new ComponentContract(componentTypes);
            if(_onComponentRemovedForGroup.TryGetValue(contract, out var existingObservable))
            { return existingObservable; }

            var newSub = new Subject<EntityChanges>();
            _onComponentRemovedForGroup.Add(contract, newSub);
            return newSub;
        }
        
        public void PublishEntityAddedComponents(int entityId, int[] componentIds)
        {
            foreach (var outstandingSubs in _onComponentAddedForGroup)
            {
                var overlap = outstandingSubs.Key.GetMatchingComponentIds(componentIds);
                if(overlap.Length == 0) { continue; }
                outstandingSubs.Value.OnNext(new EntityChanges(entityId, overlap));
            }
        }
        
        public void PublishEntityRemovingComponents(int entityId, int[] componentIds)
        {
            foreach (var outstandingSubs in _onComponentRemovingForGroup)
            {
                var overlap = outstandingSubs.Key.GetMatchingComponentIds(componentIds);
                if(overlap.Length == 0) { continue; }
                outstandingSubs.Value.OnNext(new EntityChanges(entityId, overlap));
            }
        }
        
        public void PublishEntityRemovedComponents(int entityId, int[] componentIds)
        {
            foreach (var outstandingSubs in _onComponentRemovedForGroup)
            {
                var overlap = outstandingSubs.Key.GetMatchingComponentIds(componentIds);
                if(overlap.Length == 0) { continue; }
                outstandingSubs.Value.OnNext(new EntityChanges(entityId, overlap));
            }
        }
    }
}