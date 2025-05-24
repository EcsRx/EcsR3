using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities.Registries;
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
    [Priority(10)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        private readonly object _lock = new object();
        
        public SetupSystemHandler(IComputedEntityGroupRegistry computedEntityGroupRegistry)
        {
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ISetupSystem; }

        public void SetupSystem(ISystem system)
        {
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            var entityChangeSubscriptions = new CompositeDisposable();

            lock (_lock)
            {
                _entitySubscriptions.Add(system, entitySubscriptions);
                _systemSubscriptions.Add(system, entityChangeSubscriptions);
            }

            var castSystem = (ISetupSystem) system;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);

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
                    if (entitySubscriptions.ContainsKey(x.Id))
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in observableGroup)
            { SetupEntity(castSystem, entity, entitySubscriptions); }
        }
        
        public void SetupEntity(ISetupSystem system, IEntity entity, Dictionary<int, IDisposable> subs)
        {
            lock (_lock)
            { subs.Add(entity.Id, null); }
                
            var subscription = ProcessEntity(system, entity);
            if(subscription == null) { return; }

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
            { _systemSubscriptions.RemoveAndDispose(system); }
        }

        public IDisposable ProcessEntity(ISetupSystem system, IEntity entity)
        {
            var hasEntityPredicate = system.Group is IHasPredicate;

            if (!hasEntityPredicate)
            {
                system.Setup(entity);
                return null;
            }

            var groupPredicate = system.Group as IHasPredicate;

            if (groupPredicate.CanProcessEntity(entity))
            {
                system.Setup(entity);
                return null;
            }

            var disposable = entity
                .WaitForPredicateMet(groupPredicate.CanProcessEntity)
                .ContinueWith(x =>
                {
                    _entitySubscriptions[system].Remove(x.Result.Id);
                    system.Setup(x.Result);
                });

            return disposable;
        }

        public void Dispose()
        {
            lock (_lock)
            { _systemSubscriptions.DisposeAll(); }
        }
    }
}