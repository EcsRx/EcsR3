using System;
using System.Collections.Generic;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities.Accessors;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Extensions;
using SystemsR3.Systems;

namespace EcsR3.Systems.Handlers
{
    [Priority(3)]
    public class ReactToEntitySystemHandler : IConventionalSystemHandler
    {
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IEntityComponentAccessor EntityComponentAccessor;
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        
        private readonly object _lock = new object();
        
        public ReactToEntitySystemHandler(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroupRegistry computedEntityGroupRegistry)
        {
            EntityComponentAccessor = entityComponentAccessor;
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToEntitySystem; }
        
        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToEntitySystem) system;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);            
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            var entityChangeSubscriptions = new CompositeDisposable();

            lock (_lock)
            {
                _entitySubscriptions.Add(system, entitySubscriptions);
                _systemSubscriptions.Add(system, entityChangeSubscriptions);
            }
           
            observableGroup.OnAdded
                .Subscribe(x =>
                {
                    // This occurs if we have an add elsewhere removing the entity before this one is called
                    if (observableGroup.Contains(x))
                    { SetupEntity(castSystem, x, entitySubscriptions); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnRemoved
                .Subscribe(x =>
                {
                    // This is if the add elsewhere removes the entity, which triggers this before the add is
                    if (entitySubscriptions.ContainsKey(x))
                    { entitySubscriptions.RemoveAndDispose(x); }
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in observableGroup)
            { SetupEntity(castSystem, entity, entitySubscriptions); }
        }

        public void SetupEntity(IReactToEntitySystem system, int entityId, Dictionary<int, IDisposable> subs)
        {
            lock (_lock)
            { subs.Add(entityId, null); }
                
            var subscription = ProcessEntity(system, entityId);

            lock (_lock)
            {
                if (subs.ContainsKey(entityId))
                { subs[entityId] = subscription; }
                else
                { subscription.Dispose(); }
            }
        }

        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            {
                _systemSubscriptions.RemoveAndDispose(system);
            
                var entitySubscriptions = _entitySubscriptions[system];
                entitySubscriptions.Values.DisposeAll();
                entitySubscriptions.Clear();
                _entitySubscriptions.Remove(system);
            }
        }
        
        public IDisposable ProcessEntity(IReactToEntitySystem system, int entityId)
        {
            var reactObservable = system.ReactToEntity(EntityComponentAccessor, entityId);
            return reactObservable.Subscribe(x => system.Process(EntityComponentAccessor, x));
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _systemSubscriptions.DisposeAll();
                foreach (var entitySubscriptions in _entitySubscriptions.Values)
                {
                    entitySubscriptions.Values.DisposeAll();
                    entitySubscriptions.Clear();
                }
                _entitySubscriptions.Clear();
            }
        }
    }
}