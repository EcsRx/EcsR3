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
            
            foreach (var excludedComponents in group.ExcludedComponents)
            {
                EntityChangeRouter
                    .SubscribeOnEntityAddedComponent(excludedComponents)
                    .Subscribe(OnExcludedComponentAdded)
                    .AddTo(_componentSubscriptions);
                
                EntityChangeRouter
                    .SubscribeOnEntityRemovedComponent(excludedComponents)
                    .Subscribe(OnExcludedComponentRemoved)
                    .AddTo(_componentSubscriptions);
            }
        }

        public void StartTracking(int entityId, GroupMatchingState state)
        {
            lock (_lock)
            { EntityIdMatchState.Add(entityId, state); }
            
            if (state.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityId); }
        }

        public void CheckForRemoval(int entityId, GroupMatchingState state)
        {
            if (state.NeedsComponentsAdding != Group.RequiredComponents.Length)
            { return; }
            
            EntityIdMatchState.Remove(entityId);
        }

        public GroupMatchingState GetStateSafely(int entityId)
        {
            lock (_lock)
            {
                if (EntityIdMatchState.TryGetValue(entityId, out var value))
                { return value; }
                
                var entityState = new GroupMatchingState(Group);
                EntityIdMatchState.Add(entityId, entityState);
                return entityState;
            }
        }
        
        public void OnRequiredComponentRemoving(EntityChange entityChange)
        {
            GroupMatchingState currentState;
            lock (_lock)
            { currentState = EntityIdMatchState[entityChange.EntityId]; }
            
            if(currentState.IsMatch())
            { _onEntityLeavingGroup.OnNext(entityChange.EntityId); }
        }

        public void OnRequiredComponentRemoved(EntityChange entityChange)
        {
            GroupMatchingState currentState;
            lock (_lock)
            { currentState = EntityIdMatchState[entityChange.EntityId]; }

            if (currentState.IsMatch())
            { _onEntityLeftGroup.OnNext(entityChange.EntityId); }

            lock (_lock)
            { currentState.NeedsComponentsAdding++; }
            CheckForRemoval(entityChange.EntityId, currentState);
        }

        public void OnExcludedComponentAdded(EntityChange entityChange)
        {
            var currentState = GetStateSafely(entityChange.EntityId);
            if (currentState.IsMatch())
            {
                _onEntityLeavingGroup.OnNext(entityChange.EntityId);
                _onEntityLeftGroup.OnNext(entityChange.EntityId);
            }
            
            lock(_lock)
            { currentState.NeedsComponentsRemoving++; }
        }

        public void OnExcludedComponentRemoved(EntityChange entityChange)
        {
            GroupMatchingState currentState;
            lock (_lock)
            {
                currentState = EntityIdMatchState[entityChange.EntityId];
                currentState.NeedsComponentsRemoving--;
            }
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChange.EntityId); }
        }

        public void OnRequiredComponentAdded(EntityChange entityChange)
        {
            var currentState = GetStateSafely(entityChange.EntityId);
            lock (_lock) { currentState.NeedsComponentsAdding--; }

            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChange.EntityId); }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _onEntityJoinedGroup.Dispose();
                _onEntityLeavingGroup.Dispose();
                _onEntityLeftGroup.Dispose();
            }
        }

        public IEnumerable<int> GetMatchedEntityIds()
        {
            return EntityIdMatchState
                .Where(x => x.Value.IsMatch())
                .Select(x => x.Key);
        }
    }
}