using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using R3;

namespace EcsR3.Groups.Tracking.Trackers
{
    public class ComputedEntityGroupTracker : IComputedEntityGroupTracker
    {
        public LookupGroup Group { get; }
        
        public Observable<Entity> OnEntityJoinedGroup => _onEntityJoinedGroup;
        public Observable<Entity> OnEntityLeavingGroup => _onEntityLeavingGroup;
        public Observable<Entity> OnEntityLeftGroup => _onEntityLeftGroup;
        
        public Subject<Entity> _onEntityJoinedGroup { get; } = new Subject<Entity>();
        public Subject<Entity> _onEntityLeavingGroup { get; } = new Subject<Entity>();
        public Subject<Entity> _onEntityLeftGroup { get; } = new Subject<Entity>();

        public Dictionary<Entity, GroupMatchingState> EntityIdMatchState { get; } = new Dictionary<Entity, GroupMatchingState>();
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

        public void StartTracking(Entity entity, GroupMatchingState state)
        {
            lock (_lock)
            { EntityIdMatchState.Add(entity, state); }
            
            if (state.IsMatch())
            { _onEntityJoinedGroup.OnNext(entity); }
        }

        public void CheckForRemoval(Entity entity, GroupMatchingState state)
        {
            if (state.NeedsComponentsAdding != Group.RequiredComponents.Length)
            { return; }
            
            EntityIdMatchState.Remove(entity);
        }

        public GroupMatchingState GetStateSafely(Entity entity)
        {
            lock (_lock)
            {
                if (EntityIdMatchState.TryGetValue(entity, out var value))
                { return value; }
                
                var entityState = new GroupMatchingState(Group);
                EntityIdMatchState.Add(entity, entityState);
                return entityState;
            }
        }

        public void OnRequiredComponentsAdded(EntityChanges entityChanges)
        {
            var currentState = GetStateSafely(entityChanges.Entity);
            lock (_lock) { currentState.NeedsComponentsAdding -= entityChanges.ComponentIds.Length; }
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChanges.Entity); }
        }
        
        public void OnRequiredComponentsRemoving(EntityChanges entityChanges)
        {
            GroupMatchingState currentState;
            lock (_lock)
            { currentState = EntityIdMatchState[entityChanges.Entity]; }
            
            if(currentState.IsMatch())
            { _onEntityLeavingGroup.OnNext(entityChanges.Entity); }
        }

        public void OnRequiredComponentsRemoved(EntityChanges entityChanges)
        {
            GroupMatchingState currentState;
            lock (_lock)
            { currentState = EntityIdMatchState[entityChanges.Entity]; }

            if (currentState.IsMatch())
            { _onEntityLeftGroup.OnNext(entityChanges.Entity); }

            lock (_lock)
            { currentState.NeedsComponentsAdding += entityChanges.ComponentIds.Length; }
            CheckForRemoval(entityChanges.Entity, currentState);
        }

        public void OnExcludedComponentsAdded(EntityChanges entityChanges)
        {
            var currentState = GetStateSafely(entityChanges.Entity);
            if (currentState.IsMatch())
            {
                _onEntityLeavingGroup.OnNext(entityChanges.Entity);
                _onEntityLeftGroup.OnNext(entityChanges.Entity);
            }
            
            lock(_lock)
            { currentState.NeedsComponentsRemoving += entityChanges.ComponentIds.Length; }
        }

        public void OnExcludedComponentsRemoved(EntityChanges entityChanges)
        {
            GroupMatchingState currentState;
            lock (_lock)
            {
                currentState = EntityIdMatchState[entityChanges.Entity];
                currentState.NeedsComponentsRemoving -= entityChanges.ComponentIds.Length;
            }
            
            if (currentState.IsMatch())
            { _onEntityJoinedGroup.OnNext(entityChanges.Entity); }
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

        public IEnumerable<Entity> GetMatchedEntities()
        {
            return EntityIdMatchState
                .Where(x => x.Value.IsMatch())
                .Select(x => x.Key);
        }
    }
}