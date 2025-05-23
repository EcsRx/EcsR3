using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections;
using EcsR3.Entities;
using EcsR3.Groups;
using EcsR3.Extensions;
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
        public readonly IComputedGroupManager ComputedGroupManager;
        
        private readonly object _lock = new object();
        
        public ReactToEntitySystemHandler(IComputedGroupManager computedGroupManager)
        {
            ComputedGroupManager = computedGroupManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToEntitySystem; }
        
        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToEntitySystem) system;
            var observableGroup = ComputedGroupManager.GetComputedGroup(castSystem.Group);            
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
                    if (observableGroup.Contains(x.Id))
                    { SetupEntity(castSystem, x, entitySubscriptions); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnRemoved
                .Subscribe(x =>
                {
                    // This is if the add elsewhere removes the entity, which triggers this before the add is
                    if (entitySubscriptions.ContainsKey(x.Id))
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in observableGroup.Value)
            { SetupEntity(castSystem, entity, entitySubscriptions); }
        }

        public void SetupEntity(IReactToEntitySystem system, IEntity entity, Dictionary<int, IDisposable> subs)
        {
            lock (_lock)
            { subs.Add(entity.Id, null); }
                
            var subscription = ProcessEntity(system, entity);

            lock (_lock)
            {
                if (subs.ContainsKey(entity.Id))
                { subs[entity.Id] = subscription; }
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
        
        public IDisposable ProcessEntity(IReactToEntitySystem system, IEntity entity)
        {
            var hasEntityPredicate = system.Group is IHasPredicate;
            var reactObservable = system.ReactToEntity(entity);
            
            if (!hasEntityPredicate)
            { return reactObservable.Subscribe(system.Process); }

            var groupPredicate = system.Group as IHasPredicate;
            return reactObservable?
                .Subscribe(x =>
                {
                    if(groupPredicate.CanProcessEntity(x))
                    { system.Process(x); }
                });
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