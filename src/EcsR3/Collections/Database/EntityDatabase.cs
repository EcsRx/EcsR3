using System;
using System.Collections.Generic;
using EcsR3.Collections.Entity;
using EcsR3.Collections.Events;
using EcsR3.Lookups;
using SystemsR3.Extensions;
using R3;

namespace EcsR3.Collections.Database
{
    public class EntityDatabase : IEntityDatabase
    {
        private readonly CollectionLookup _collections;
        private readonly IDictionary<int, IDisposable> _collectionSubscriptions;
        private readonly object _lock = new object();

        public IReadOnlyList<IEntityCollection> Collections => _collections;
        public IEntityCollection this[int id] => _collections[id];

        public IEntityCollectionFactory EntityCollectionFactory { get; }
        
        public Observable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public Observable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        public Observable<IEntityCollection> CollectionAdded => _onCollectionAdded;
        public Observable<IEntityCollection> CollectionRemoved => _onCollectionRemoved;

        private readonly Subject<IEntityCollection> _onCollectionAdded;
        private readonly Subject<IEntityCollection> _onCollectionRemoved;
        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;

        public EntityDatabase(IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionFactory = entityCollectionFactory;

            _collections = new CollectionLookup();
            _collectionSubscriptions = new Dictionary<int, IDisposable>();
            _onCollectionAdded = new Subject<IEntityCollection>();
            _onCollectionRemoved = new Subject<IEntityCollection>();
            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();

            CreateCollection(EntityCollectionLookups.DefaultCollectionId);
        }

        public void SubscribeToCollection(IEntityCollection collection)
        {
            lock (_lock)
            {
                var collectionDisposable = new CompositeDisposable();   
                collection.EntityAdded.Subscribe(x => _onEntityAdded.OnNext(x)).AddTo(collectionDisposable);
                collection.EntityRemoved.Subscribe(x => _onEntityRemoved.OnNext(x)).AddTo(collectionDisposable);

                _collectionSubscriptions.Add(collection.Id, collectionDisposable);
            }
        }

        public void UnsubscribeFromCollection(int id)
        {
            lock (_lock)
            { _collectionSubscriptions.RemoveAndDispose(id); }
        }
        
        public IEntityCollection CreateCollection(int id)
        {
            var collection = EntityCollectionFactory.Create(id);
            AddCollection(collection);
            return collection;
        }
        
        public void AddCollection(IEntityCollection collection)
        {
            lock (_lock)
            {
                _collections.Add(collection);
                SubscribeToCollection(collection);
            }

            _onCollectionAdded.OnNext(collection);
        }

        public IEntityCollection GetCollection(int id = EntityCollectionLookups.DefaultCollectionId)
        { return _collections.Contains(id) ? _collections[id] : null; }

        public void RemoveCollection(int id, bool disposeEntities = true)
        {
            IEntityCollection removedCollection;
            lock (_lock)
            {
                if(!_collections.Contains(id)) { return; }

                removedCollection = _collections[id];
                _collections.Remove(id);
            
                UnsubscribeFromCollection(id);
            }

            _onCollectionRemoved.OnNext(removedCollection);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _onEntityAdded.Dispose();
                _onEntityRemoved.Dispose();
            
                _collectionSubscriptions.RemoveAndDisposeAll();
            }
        }
    }
}