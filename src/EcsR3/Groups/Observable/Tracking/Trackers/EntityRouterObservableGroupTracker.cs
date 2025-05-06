using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities.Routing;
using R3;

namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public class EntityRouterObservableGroupTracker : IObservableGroupTracker
    {
        public LookupGroup Group { get; }
        
        public Observable<int> OnEntityJoinedGroup => _onEntityJoinedGroup;
        public Observable<int> OnEntityLeavingGroup => _onEntityLeavingGroup;
        public Observable<int> OnEntityLeftGroup => _onEntityLeftGroup;
        
        public Subject<int> _onEntityJoinedGroup { get; } = new Subject<int>();
        public Subject<int> _onEntityLeavingGroup { get; } = new Subject<int>();
        public Subject<int> _onEntityLeftGroup { get; } = new Subject<int>();


        public Dictionary<int, GroupMatchingState> EntityIdMatchState { get; } = new Dictionary<int, GroupMatchingState>();
        public IEntityChangeRouter EntityChangeRouter { get; }

        private readonly object _lock = new object();
        private readonly CompositeDisposable _componentSubscriptions = new CompositeDisposable();

        public EntityRouterObservableGroupTracker(IEntityChangeRouter entityChangeRouter, LookupGroup group)
        {
            Group = group;
            EntityChangeRouter = entityChangeRouter;

            foreach (var requiredComponent in group.RequiredComponents)
            {
                EntityChangeRouter
                    .SubscribeOnEntityAddedComponent(requiredComponent)
                    .Subscribe(OnRequiredComponentAdded)
                    .AddTo(_componentSubscriptions);
                
                EntityChangeRouter
                    .SubscribeOnEntityRemovingComponent(requiredComponent)
                    .Subscribe(OnRequiredComponentRemoving)
                    .AddTo(_componentSubscriptions);
                
                EntityChangeRouter
                    .SubscribeOnEntityRemovedComponent(requiredComponent)
                    .Subscribe(OnRequiredComponentRemoved)
                    .AddTo(_componentSubscriptions);
            }
            
            foreach (var requiredComponent in group.ExcludedComponents)
            {
                EntityChangeRouter
                    .SubscribeOnEntityAddedComponent(requiredComponent)
                    .Subscribe(OnExcludedComponentAdded)
                    .AddTo(_componentSubscriptions);
                
                EntityChangeRouter
                    .SubscribeOnEntityRemovedComponent(requiredComponent)
                    .Subscribe(OnExcludedComponentRemoved)
                    .AddTo(_componentSubscriptions);
            }
        }

        public void StartTracking(int entityId, GroupMatchingState state)
        {
            EntityIdMatchState.Add(entityId, state);
            
            if (state.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityId); }
        }

        public GroupMatchingState GetStateSafely(int entityId)
        {
            if (EntityIdMatchState.TryGetValue(entityId, out var value))
            { return value; }

            var entityState = new GroupMatchingState(Group);
            EntityIdMatchState.Add(entityId, entityState);
            return entityState;
        }
        
        public void OnRequiredComponentRemoving(EntityChange entityChange)
        {
            var currentState = EntityIdMatchState[entityChange.EntityId];
            if(currentState.IsMatch())
            { _onEntityLeavingGroup.OnNext(entityChange.EntityId); }
        }

        public void OnRequiredComponentRemoved(EntityChange entityChange)
        {
            var currentState = EntityIdMatchState[entityChange.EntityId];
            if (!currentState.IsMatch()) { return; }
            
            currentState.NeedsComponentsAdding++;
            _onEntityLeftGroup.OnNext(entityChange.EntityId);
        }

        public void OnExcludedComponentAdded(EntityChange entityChange)
        {
            var currentState = GetStateSafely(entityChange.EntityId);
            if (currentState.IsMatch())
            {
                _onEntityLeavingGroup.OnNext(entityChange.EntityId);
                _onEntityLeftGroup.OnNext(entityChange.EntityId);
            }
            currentState.NeedsComponentsRemoving++;
        }

        public void OnExcludedComponentRemoved(EntityChange entityChange)
        {
            var currentState = EntityIdMatchState[entityChange.EntityId];
            currentState.NeedsComponentsRemoving--;
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChange.EntityId); }
        }

        public void OnRequiredComponentAdded(EntityChange entityChange)
        {
            var currentState = GetStateSafely(entityChange.EntityId);
            currentState.NeedsComponentsAdding--;
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChange.EntityId); }
        }

        public void Dispose()
        {
            _onEntityJoinedGroup.Dispose();
            _onEntityLeavingGroup.Dispose();
            _onEntityLeftGroup.Dispose();
        }

        public IEnumerable<int> GetMatchedEntityIds()
        {
            return EntityIdMatchState
                .Where(x => x.Value.IsMatch())
                .Select(x => x.Key);
        }
    }
}