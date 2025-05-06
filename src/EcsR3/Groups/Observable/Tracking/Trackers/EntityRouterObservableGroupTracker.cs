using System.Collections.Generic;
using System.Linq;
using EcsR3.Entities.Routing;
using EcsR3.Groups.Observable.Tracking.Events;
using EcsR3.Groups.Observable.Tracking.Types;
using R3;

namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public class EntityRouterObservableGroupTracker : IObservableGroupTracker
    {
        public LookupGroup Group { get; }

        public Dictionary<int, GroupMatchingState> EntityIdMatchState { get; } = new Dictionary<int, GroupMatchingState>();
        
        public Subject<EntityGroupStateChanged> OnGroupMatchingChanged { get; }
        public Observable<EntityGroupStateChanged> GroupMatchingChanged => OnGroupMatchingChanged;
        
        public IEntityChangeRouter EntityChangeRouter { get; }

        private readonly object _lock = new object();
        private readonly CompositeDisposable _componentSubscriptions = new CompositeDisposable();

        public EntityRouterObservableGroupTracker(IEntityChangeRouter entityChangeRouter, LookupGroup group)
        {
            Group = group;
            EntityChangeRouter = entityChangeRouter;
            OnGroupMatchingChanged = new Subject<EntityGroupStateChanged>();

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
            { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityId, GroupActionType.JoinedGroup)); }
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
            { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityChange.EntityId, GroupActionType.LeavingGroup)); }
        }

        public void OnRequiredComponentRemoved(EntityChange entityChange)
        {
            var currentState = EntityIdMatchState[entityChange.EntityId];
            if (!currentState.IsMatch()) { return; }
            
            currentState.NeedsComponentsAdding++;
            OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityChange.EntityId, GroupActionType.LeftGroup));
        }

        public void OnExcludedComponentAdded(EntityChange entityChange)
        {
            var currentState = GetStateSafely(entityChange.EntityId);
            if (currentState.IsMatch())
            {
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityChange.EntityId, GroupActionType.LeavingGroup));
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityChange.EntityId, GroupActionType.LeftGroup));
            }
            currentState.NeedsComponentsRemoving++;
        }

        public void OnExcludedComponentRemoved(EntityChange entityChange)
        {
            var currentState = EntityIdMatchState[entityChange.EntityId];
            currentState.NeedsComponentsRemoving--;
            
            if (currentState.IsMatch())
            { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityChange.EntityId, GroupActionType.JoinedGroup)); }
        }

        public void OnRequiredComponentAdded(EntityChange entityChange)
        {
            var currentState = GetStateSafely(entityChange.EntityId);
            currentState.NeedsComponentsAdding--;
            
            if (currentState.IsMatch())
            { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entityChange.EntityId, GroupActionType.JoinedGroup)); }
        }
        
        public void Dispose()
        { OnGroupMatchingChanged.Dispose(); }

        public IEnumerable<int> GetMatchedEntityIds()
        {
            return EntityIdMatchState
                .Where(x => x.Value.IsMatch())
                .Select(x => x.Key);
        }
    }
}