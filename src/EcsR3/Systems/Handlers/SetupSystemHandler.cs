using System;
using System.Collections.Generic;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities.Accessors;
using EcsR3.Systems.Reactive;
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
        public readonly IEntityComponentAccessor EntityComponentAccessor;
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        private readonly object _lock = new object();
        
        public SetupSystemHandler(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroupRegistry computedEntityGroupRegistry)
        {
            EntityComponentAccessor = entityComponentAccessor;
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
                    if (observableGroup.Contains(x))
                    { SetupEntity(castSystem, x, entitySubscriptions); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnRemoved
                .Subscribe(x =>
                {
                    if (entitySubscriptions.ContainsKey(x))
                    { entitySubscriptions.RemoveAndDispose(x); }
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in observableGroup)
            { SetupEntity(castSystem, entity, entitySubscriptions); }
        }
        
        public void SetupEntity(ISetupSystem system, int entity, Dictionary<int, IDisposable> subs)
        {
            lock (_lock)
            { subs.Add(entity, null); }
                
            ProcessEntity(system, entity);
        }

        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            { _systemSubscriptions.RemoveAndDispose(system); }
        }

        public void ProcessEntity(ISetupSystem system, int entity)
        {
            system.Setup(EntityComponentAccessor, entity);
        }

        public void Dispose()
        {
            lock (_lock)
            { _systemSubscriptions.DisposeAll(); }
        }
    }
}