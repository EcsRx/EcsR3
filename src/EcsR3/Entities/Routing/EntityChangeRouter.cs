using System.Collections.Generic;
using EcsR3.Components.Lookups;
using R3;
using SystemsR3.Extensions;

namespace EcsR3.Entities.Routing
{
    public class EntityChangeRouter : IEntityChangeRouter
    {
        private readonly Dictionary<int, Subject<EntityChange>> _onEntityAddedComponent;
        private readonly Dictionary<int, Subject<EntityChange>> _onEntityRemovingComponent;
        private readonly Dictionary<int, Subject<EntityChange>> _onEntityComponentRemoved;
        
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public EntityChangeRouter(IComponentTypeLookup componentTypeLookup)
        {
            ComponentTypeLookup = componentTypeLookup;
            
            _onEntityAddedComponent = new Dictionary<int, Subject<EntityChange>>();
            _onEntityRemovingComponent = new Dictionary<int, Subject<EntityChange>>();
            _onEntityComponentRemoved = new Dictionary<int, Subject<EntityChange>>();
            
            CreateComponentStreams();
        }

        public void CreateComponentStreams()
        {
            foreach (var componentType in ComponentTypeLookup.AllComponentTypeIds)
            {
                _onEntityAddedComponent.Add(componentType, new Subject<EntityChange>());;
                _onEntityRemovingComponent.Add(componentType, new Subject<EntityChange>());;
                _onEntityComponentRemoved.Add(componentType, new Subject<EntityChange>());;
            }
        }

        public void Dispose()
        {
            _onEntityAddedComponent.ForEachRun(x => x.Value.Dispose());
            _onEntityRemovingComponent.ForEachRun(x => x.Value.Dispose());
            _onEntityComponentRemoved.ForEachRun(x => x.Value.Dispose());
        }

        public Observable<EntityChange> OnEntityAddedComponent(int componentType) => _onEntityAddedComponent[componentType];
        public Observable<EntityChange> OnEntityRemovingComponent(int componentType) => _onEntityRemovingComponent[componentType];
        public Observable<EntityChange> OnEntityRemovedComponent(int componentType) => _onEntityComponentRemoved[componentType];
        
        public void PublishEntityAddedComponent(int entityId, int componentId) => _onEntityAddedComponent[componentId].OnNext(new EntityChange(entityId, componentId));
        public void PublishEntityRemovingComponent(int entityId, int componentId) => _onEntityRemovingComponent[componentId].OnNext(new EntityChange(entityId, componentId));
        public void PublishEntityRemovedComponent(int entityId, int componentId) => _onEntityComponentRemoved[componentId].OnNext(new EntityChange(entityId, componentId));
    }
}