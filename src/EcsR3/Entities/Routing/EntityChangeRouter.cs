using System;
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
        { ComponentTypeLookup = componentTypeLookup; }

        public void Dispose()
        {
            _onComponentAddedForGroup.ForEachRun(x => x.Value.Dispose());
            _onComponentRemovingForGroup.ForEachRun(x => x.Value.Dispose());
            _onComponentRemovedForGroup.ForEachRun(x => x.Value.Dispose());
        }

        public Observable<EntityChanges> OnEntityAddedComponents(params int[] componentTypes)
        { return OnEntityComponentEvent(_onComponentAddedForGroup, componentTypes); }
        
        public Observable<EntityChanges> OnEntityRemovingComponents(params int[] componentTypes)
        { return OnEntityComponentEvent(_onComponentRemovingForGroup, componentTypes); }
        
        public Observable<EntityChanges> OnEntityRemovedComponents(params int[] componentTypes)
        { return OnEntityComponentEvent(_onComponentRemovedForGroup, componentTypes); }
        
        public void PublishEntityAddedComponents(int entityId, int[] componentIds)
        { PublishEntityComponentEvent(entityId, componentIds, _onComponentAddedForGroup); }
        
        public void PublishEntityRemovingComponents(int entityId, int[] componentIds)
        { PublishEntityComponentEvent(entityId, componentIds, _onComponentRemovingForGroup); }
        
        public void PublishEntityRemovedComponents(int entityId, int[] componentIds)
        { PublishEntityComponentEvent(entityId, componentIds, _onComponentRemovedForGroup); }
        
        public Observable<EntityChanges> OnEntityComponentEvent(Dictionary<ComponentContract, Subject<EntityChanges>> source, params int[] componentTypes)
        {
            var contract = new ComponentContract(componentTypes);
            if(source.TryGetValue(contract, out var existingObservable))
            { return existingObservable; }

            var newSub = new Subject<EntityChanges>();
            source.Add(contract, newSub);
            return newSub;
        }

        public void PublishEntityComponentEvent(int entityId, int[] componentIds, Dictionary<ComponentContract, Subject<EntityChanges>> source)
        {
            var buffer = new int[componentIds.Length];
            foreach (var outstandingSubs in source)
            {
                var lastUsedIndexInBuffer = outstandingSubs.Key.GetMatchingComponentIdsNoAlloc(componentIds, buffer);
                if(lastUsedIndexInBuffer == 0) { continue; }

                ReadOnlyMemory<int> bufferAsMemory = buffer;
                outstandingSubs.Value.OnNext(new EntityChanges(entityId, bufferAsMemory[..lastUsedIndexInBuffer]));
            }
        }
    }
}