using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities.Routing;
using R3;

namespace EcsR3.Groups.Tracking.Trackers
{
    public class ComputedEntityGroupTracker : IComputedEntityGroupTracker
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

        public ComputedEntityGroupTracker(IEntityChangeRouter entityChangeRouter, LookupGroup group)
        {
            Group = group;
            EntityChangeRouter = entityChangeRouter;

            EntityChangeRouter
                .OnEntityAddedComponents(group.RequiredComponents)
                .Subscribe(OnRequiredComponentsAdded)
                .AddTo(_componentSubscriptions);
            
            EntityChangeRouter
                .OnEntityRemovingComponents(group.RequiredComponents)
                .Subscribe(OnRequiredComponentsRemoving)
                .AddTo(_componentSubscriptions);
            
            EntityChangeRouter
                .OnEntityRemovedComponents(group.RequiredComponents)
                .Subscribe(OnRequiredComponentsRemoved)
                .AddTo(_componentSubscriptions);

            if (group.ExcludedComponents.Length == 0) { return; }
            
            EntityChangeRouter
                .OnEntityAddedComponents(group.ExcludedComponents)
                .Subscribe(OnExcludedComponentsAdded)
                .AddTo(_componentSubscriptions);
            
            EntityChangeRouter
                .OnEntityRemovedComponents(group.ExcludedComponents)
                .Subscribe(OnExcludedComponentsRemoved)
                .AddTo(_componentSubscriptions);
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

        public void OnRequiredComponentsAdded(EntityChanges entityChanges)
        {
            var currentState = GetStateSafely(entityChanges.EntityId);
            lock (_lock) { currentState.NeedsComponentsAdding -= entityChanges.ComponentIds.Length; }
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChanges.EntityId); }
        }
        
        public void OnRequiredComponentsRemoving(EntityChanges entityChanges)
        {
            GroupMatchingState currentState;
            lock (_lock)
            { currentState = EntityIdMatchState[entityChanges.EntityId]; }
            
            if(currentState.IsMatch())
            { _onEntityLeavingGroup.OnNext(entityChanges.EntityId); }
        }

        public void OnRequiredComponentsRemoved(EntityChanges entityChanges)
        {
            GroupMatchingState currentState;
            lock (_lock)
            { currentState = EntityIdMatchState[entityChanges.EntityId]; }

            if (currentState.IsMatch())
            { _onEntityLeftGroup.OnNext(entityChanges.EntityId); }

            lock (_lock)
            { currentState.NeedsComponentsAdding += entityChanges.ComponentIds.Length; }
            CheckForRemoval(entityChanges.EntityId, currentState);
        }

        public void OnExcludedComponentsAdded(EntityChanges entityChanges)
        {
            var currentState = GetStateSafely(entityChanges.EntityId);
            if (currentState.IsMatch())
            {
                _onEntityLeavingGroup.OnNext(entityChanges.EntityId);
                _onEntityLeftGroup.OnNext(entityChanges.EntityId);
            }
            
            lock(_lock)
            { currentState.NeedsComponentsRemoving += entityChanges.ComponentIds.Length; }
        }

        public void OnExcludedComponentsRemoved(EntityChanges entityChanges)
        {
            GroupMatchingState currentState;
            lock (_lock)
            {
                currentState = EntityIdMatchState[entityChanges.EntityId];
                currentState.NeedsComponentsRemoving -= entityChanges.ComponentIds.Length;
            }
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChanges.EntityId); }
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