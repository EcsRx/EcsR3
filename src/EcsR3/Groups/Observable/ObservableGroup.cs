using System;
using System.Collections;
using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Groups.Observable.Tracking.Events;
using EcsR3.Groups.Observable.Tracking.Trackers;
using EcsR3.Groups.Observable.Tracking.Types;
using EcsR3.Lookups;
using SystemsR3.Extensions;
using R3;

namespace EcsR3.Groups.Observable
{
    public class ObservableGroup : IObservableGroup
    {
        public readonly EntityLookup CachedEntities;
        public readonly List<IDisposable> Subscriptions;

        public Observable<IEntity> OnEntityAdded => _onEntityAdded;
        public Observable<IEntity> OnEntityRemoved => _onEntityRemoved;
        public Observable<IEntity> OnEntityRemoving => _onEntityRemoving;
        public ICollectionObservableGroupTracker GroupTracker { get; }

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        private readonly Subject<IEntity> _onEntityRemoving;
        
        public ObservableGroupToken Token { get; }
        
        public ObservableGroup(ObservableGroupToken token, IEnumerable<IEntity> initialEntities, ICollectionObservableGroupTracker tracker)
        {
            Token = token;
            GroupTracker = tracker;
            
            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new List<IDisposable>();
            CachedEntities = new EntityLookup();

            GroupTracker.GroupMatchingChanged
                .Subscribe(OnEntityGroupChanged)
                .AddTo(Subscriptions);

            foreach (var entity in initialEntities)
            {
                var currentlyMatches = GroupTracker.IsMatching(entity.Id);
                if(currentlyMatches) { CachedEntities.Add(entity); }
            }
        }
        
        public void OnEntityGroupChanged(EntityGroupStateChanged args)
        {
            if (args.GroupActionType == GroupActionType.JoinedGroup)
            {
                CachedEntities.Add(args.Entity);
                _onEntityAdded.OnNext(args.Entity);
                return;
            }

            if (args.GroupActionType == GroupActionType.LeavingGroup)
            { _onEntityRemoving.OnNext(args.Entity); }

            if (args.GroupActionType == GroupActionType.LeftGroup)
            {
                CachedEntities.Remove(args.Entity.Id);
                _onEntityRemoved.OnNext(args.Entity);
            }
        }
        
        public bool ContainsEntity(int id)
        { return CachedEntities.Contains(id); }
        
        public IEntity GetEntity(int id)
        { return CachedEntities[id]; }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            GroupTracker.Dispose();
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
            _onEntityRemoving.Dispose();
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return CachedEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public int Count => CachedEntities.Count;

        public IEntity this[int index] => CachedEntities.GetByIndex(index);
    }
}