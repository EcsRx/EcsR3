using System;
using System.Collections.Generic;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Extensions;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Extensions;
using SystemsR3.Systems;

namespace EcsR3.Systems.Handlers
{
    [Priority(10)]
    public class TeardownSystemHandler : IConventionalSystemHandler
    {
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        public readonly IDictionary<ISystem, IDisposable> SystemSubscriptions;
        private readonly object _lock = new object();
        
        public TeardownSystemHandler(IComputedEntityGroupRegistry computedEntityGroupRegistry)
        {
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            SystemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem groupSystem)
        { return groupSystem is ITeardownSystem; }

        public void SetupSystem(ISystem system)
        {
            var entityChangeSubscriptions = new CompositeDisposable();

            lock (_lock)
            { SystemSubscriptions.Add(system, entityChangeSubscriptions); }

            var castSystem = (ITeardownSystem) system;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);
            
            observableGroup.OnRemoving
                .Subscribe(castSystem.Teardown)
                .AddTo(entityChangeSubscriptions);        
        }

        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            { SystemSubscriptions.RemoveAndDispose(system); }
        }       

        public void Dispose()
        {
            lock (_lock)
            { SystemSubscriptions.DisposeAll(); }
        }
    }
}