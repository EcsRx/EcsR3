using System;
using System.Collections;
using System.Collections.Generic;
using EcsR3.Blueprints;
using EcsR3.Collections.Events;
using EcsR3.Entities;
using EcsR3.Lookups;
using R3;
using SystemsR3.Extensions;

namespace EcsR3.Collections.Entity
{
public class EntityCollection : IEntityCollection, IDisposable
    {
        public int Id { get; }
        public IEntityFactory EntityFactory { get; }
        
        public readonly EntityLookup EntityLookup;
        public readonly IDictionary<int, IDisposable> EntitySubscriptions;

        public Observable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public Observable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        public Observable<ComponentsChangedEvent> EntityComponentsAdded => _onEntityComponentsAdded;
        public Observable<ComponentsChangedEvent> EntityComponentsRemoving => _onEntityComponentsRemoving;
        public Observable<ComponentsChangedEvent> EntityComponentsRemoved => _onEntityComponentsRemoved;
        
        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsAdded;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoving;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoved;
        
        private readonly object _lock = new object();
        
        public EntityCollection(int id, IEntityFactory entityFactory)
        {
            EntityLookup = new EntityLookup();
            EntitySubscriptions = new Dictionary<int, IDisposable>();
            Id = id;
            EntityFactory = entityFactory;

            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();
            _onEntityComponentsAdded = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoving = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoved = new Subject<ComponentsChangedEvent>();
        }

        public void SubscribeToEntity(IEntity entity)
        {
            lock (_lock)
            {
                var entityDisposable = new CompositeDisposable();
                entity.ComponentsAdded.Subscribe(x => _onEntityComponentsAdded.OnNext(new ComponentsChangedEvent(entity, x))).AddTo(entityDisposable);
                entity.ComponentsRemoving.Subscribe(x => _onEntityComponentsRemoving.OnNext(new ComponentsChangedEvent(entity, x))).AddTo(entityDisposable);
                entity.ComponentsRemoved.Subscribe(x => _onEntityComponentsRemoved.OnNext(new ComponentsChangedEvent(entity, x))).AddTo(entityDisposable);
                EntitySubscriptions.Add(entity.Id, entityDisposable);
            }
        }

        public void UnsubscribeFromEntity(int entityId)
        {
            lock (_lock)
            { EntitySubscriptions.RemoveAndDispose(entityId); }
        }
        
        public IEntity CreateEntity(IBlueprint blueprint = null, int? id = null)
        {
            IEntity entity;
            lock (_lock)
            {
                if (id.HasValue && EntityLookup.Contains(id.Value))
                { throw new InvalidOperationException("id already exists"); }

                entity = EntityFactory.Create(id);

                EntityLookup.Add(entity);
                SubscribeToEntity(entity);
            }

            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
            blueprint?.Apply(entity);
            
            return entity;
        }

        public IEntity GetEntity(int id)
        { return EntityLookup[id]; }

        public void RemoveEntity(int id, bool disposeOnRemoval = true)
        {
            IEntity entity;
            lock (_lock)
            {
                entity = GetEntity(id);
                EntityLookup.Remove(id);

                var entityId = entity.Id;

                if (disposeOnRemoval)
                {
                    entity.Dispose();
                    EntityFactory.Destroy(entityId);
                }
            
                UnsubscribeFromEntity(entityId);
            }
            
            _onEntityRemoved.OnNext(new CollectionEntityEvent(entity));
        }

        public void AddEntity(IEntity entity)
        {
            lock (_lock)
            {
                EntityLookup.Add(entity);
                SubscribeToEntity(entity);
            }
            
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
        }

        public bool ContainsEntity(int id)
        {
            lock (_lock)
            { return EntityLookup.Contains(id); }
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return EntityLookup.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            lock (_lock)
            {
                _onEntityAdded.Dispose();
                _onEntityRemoved.Dispose();
                _onEntityComponentsAdded.Dispose();
                _onEntityComponentsRemoving.Dispose();
                _onEntityComponentsRemoved.Dispose();

                EntityLookup.Clear();
                EntitySubscriptions.RemoveAndDisposeAll();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                { return EntityLookup.Count; }
            }
        }

        public IEntity this[int index]
        {
            get
            {
                lock (_lock)
                { return EntityLookup.GetByIndex(index); }
            }
        }
    }
}
