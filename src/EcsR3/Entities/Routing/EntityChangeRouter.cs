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
        
        public void PublishEntityAddedComponents(Entity entity, int[] componentIds)
        { PublishEntityComponentEvent(entity, componentIds, _onComponentAddedForGroup); }
        
        public void PublishEntityRemovingComponents(Entity entity, int[] componentIds)
        { PublishEntityComponentEvent(entity, componentIds, _onComponentRemovingForGroup); }
        
        public void PublishEntityRemovedComponents(Entity[] entities, int[] componentIds)
        { PublishEntityComponentEvent(entities, componentIds, _onComponentRemovedForGroup); }
        
        public void PublishEntityAddedComponents(Entity[] entities, int[] componentIds)
        { PublishEntityComponentEvent(entities, componentIds, _onComponentAddedForGroup); }
        
        public void PublishEntityRemovingComponents(Entity[] entities, int[] componentIds)
        { PublishEntityComponentEvent(entities, componentIds, _onComponentRemovingForGroup); }
        
        public void PublishEntityRemovedComponents(Entity entity, int[] componentIds)
        { PublishEntityComponentEvent(entity, componentIds, _onComponentRemovedForGroup); }
        
        public Observable<EntityChanges> OnEntityComponentEvent(Dictionary<ComponentContract, Subject<EntityChanges>> source, params int[] componentTypes)
        {
            var contract = new ComponentContract(componentTypes);
            if(source.TryGetValue(contract, out var existingObservable))
            { return existingObservable; }

            var newSub = new Subject<EntityChanges>();
            source.Add(contract, newSub);
            return newSub;
        }

        public void PublishEntityComponentEvent(Entity entity, int[] componentIds, Dictionary<ComponentContract, Subject<EntityChanges>> source)
        {
            var buffer = new int[componentIds.Length];
            foreach (var outstandingSubs in source)
            {
                var lastUsedIndexInBuffer = outstandingSubs.Key.GetMatchingComponentIdsNoAlloc(componentIds, buffer);
                if(lastUsedIndexInBuffer == -1) { continue; }

                /*
                 This is an optimization as we know the EntityRouterComputedEntityGroupTracker subscriber will instantly 
                 use the data as its called synchronously so we can use Memory<T> and it can be re-used.
                 
                 There is a worry that anyone could subscribe elsewhere and want to use the data later but they would
                 need to convert it to an array in that scenario, if they didnt they would just have garbage data.
                 */
                ReadOnlyMemory<int> bufferAsMemory = buffer;
                outstandingSubs.Value.OnNext(new EntityChanges(entity, bufferAsMemory[..(lastUsedIndexInBuffer+1)]));
            }
        }
        
        public void PublishEntityComponentEvent(Entity[] entities, int[] componentIds, Dictionary<ComponentContract, Subject<EntityChanges>> source)
        {
            var buffer = new int[componentIds.Length];
            foreach (var outstandingSubs in source)
            {
                var lastUsedIndexInBuffer = outstandingSubs.Key.GetMatchingComponentIdsNoAlloc(componentIds, buffer);
                if(lastUsedIndexInBuffer == -1) { continue; }

                /*
                 This is an optimization as we know the EntityRouterComputedEntityGroupTracker subscriber will instantly
                 use the data as its called synchronously so we can use Memory<T> and it can be re-used.

                 There is a worry that anyone could subscribe elsewhere and want to use the data later but they would
                 need to convert it to an array in that scenario, if they didnt they would just have garbage data.
                 */
                ReadOnlyMemory<int> bufferAsMemory = buffer;
                for (var i = 0; i < entities.Length; i++)
                { outstandingSubs.Value.OnNext(new EntityChanges(entities[i], bufferAsMemory[..(lastUsedIndexInBuffer+1)])); }
            }
        }
    }
}