using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components.Lookups;
using EcsR3.Groups;
using R3;
using SystemsR3.Extensions;

namespace EcsR3.Entities.Routing
{
    public readonly struct ComponentContract : IEquatable<ComponentContract>
    {
        public readonly int[] ComponentIds;

        public ComponentContract(int[] componentIds)
        {
            ComponentIds = componentIds;
        }

        public int[] GetMatchingComponentIds(int[] comparingComponentIds)
        {
            var result = new List<int>();
            for (var i = 0; i < ComponentIds.Length; i++)
            {
                var requiredComponentId = ComponentIds[i];
                for (var j = 0; j < comparingComponentIds.Length; j++)
                {
                    if(requiredComponentId == comparingComponentIds[i])
                    { 
                        result.Add(requiredComponentId); 
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        public bool Equals(ComponentContract other)
        {
            return Equals(ComponentIds, other.ComponentIds);
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentContract other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (ComponentIds != null ? ComponentIds.GetHashCode() : 0);
        }

        public static bool operator ==(ComponentContract left, ComponentContract right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ComponentContract left, ComponentContract right)
        {
            return !left.Equals(right);
        }
    }
    
    public class EntityChangeRouter : IEntityChangeRouter
    {
        private readonly Subject<EntityChange>[] _onEntityAddedComponent;
        private readonly Subject<EntityChange>[] _onEntityRemovingComponent;
        private readonly Subject<EntityChange>[] _onEntityComponentRemoved;
        
        private readonly Dictionary<ComponentContract, Subject<EntityChanges>> _onComponentAddedForGroup = new Dictionary<ComponentContract, Subject<EntityChanges>>();
        
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public EntityChangeRouter(IComponentTypeLookup componentTypeLookup)
        {
            ComponentTypeLookup = componentTypeLookup;
            
            _onEntityAddedComponent = new Subject<EntityChange>[ComponentTypeLookup.AllComponentTypeIds.Length];
            _onEntityRemovingComponent = new Subject<EntityChange>[ComponentTypeLookup.AllComponentTypeIds.Length];
            _onEntityComponentRemoved = new Subject<EntityChange>[ComponentTypeLookup.AllComponentTypeIds.Length];
            
            foreach (var componentType in ComponentTypeLookup.AllComponentTypeIds)
            {
                _onEntityAddedComponent[componentType] = new Subject<EntityChange>();
                _onEntityRemovingComponent[componentType] = new Subject<EntityChange>();
                _onEntityComponentRemoved[componentType] = new Subject<EntityChange>();
            }
        }

        public void Dispose()
        {
            _onEntityAddedComponent.ForEachRun(x => x.Dispose());
            _onEntityRemovingComponent.ForEachRun(x => x.Dispose());
            _onEntityComponentRemoved.ForEachRun(x => x.Dispose());
        }

        public Observable<EntityChange> OnEntityAddedComponent(int componentType) => _onEntityAddedComponent[componentType];
        
        public Observable<EntityChange> OnEntityAddedComponent(params int[] componentTypes)
        { return Observable.Merge(componentTypes.Select(x => _onEntityAddedComponent[x])); }

        public Observable<EntityChanges> OnEntityAddedComponents(params int[] componentTypes)
        {
            var contract = new ComponentContract(componentTypes);
            if(_onComponentAddedForGroup.TryGetValue(contract, out var existingObservable))
            { return existingObservable; }

            var newSub = new Subject<EntityChanges>();
            _onComponentAddedForGroup.Add(contract, newSub);
            return newSub;
        }
        
        public Observable<EntityChange> OnEntityRemovingComponent(int componentType) => _onEntityRemovingComponent[componentType];
        public Observable<EntityChange> OnEntityRemovedComponent(int componentType) => _onEntityComponentRemoved[componentType];
        
        public void PublishEntityAddedComponent(int entityId, int componentId) => _onEntityAddedComponent[componentId].OnNext(new EntityChange(entityId, componentId));
        public void PublishEntityAddedComponents(int entityId, int[] componentIds)
        {
            foreach (var outstandingSubs in _onComponentAddedForGroup)
            {
                var overlap = outstandingSubs.Key.GetMatchingComponentIds(componentIds);
                if(overlap.Length == 0) { continue; }
                outstandingSubs.Value.OnNext(new EntityChanges(entityId, overlap));
            }
            
            //var matchingObservables = _onComponentAddedForGroup.Where(x => componentIds.All(y => x.Key.Contains(y)));
            //matchingObservables.ForEachRun(x => x.Value.OnNext(new EntityChanges(entityId, componentIds)));
        }

        public void PublishEntityRemovingComponent(int entityId, int componentId) => _onEntityRemovingComponent[componentId].OnNext(new EntityChange(entityId, componentId));
        public void PublishEntityRemovedComponent(int entityId, int componentId) => _onEntityComponentRemoved[componentId].OnNext(new EntityChange(entityId, componentId));
    }
}