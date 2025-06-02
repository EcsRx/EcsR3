using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Systems.Augments;
using EcsR3.Systems.Reactive;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Extensions;
using SystemsR3.Systems;
using SystemsR3.Threading;

namespace EcsR3.Systems.Handlers
{
    [Priority(6)]
    public class ReactToGroupBatchedSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityComponentAccessor EntityComponentAccessor;
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;       
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        private readonly object _lock = new object();
        
        public ReactToGroupBatchedSystemHandler(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroupRegistry computedEntityGroupRegistry)
        {
            EntityComponentAccessor = entityComponentAccessor;
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToGroupBatchedSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToGroupBatchedSystem)system;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);
            var reactObservable = castSystem.ReactToGroup(observableGroup);
                
            var noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.ToArray(), castSystem));
           
            lock (_lock)
            { _systemSubscriptions.Add(system, noPredicateSub); }
        }

        private void ExecuteForGroup(IReadOnlyList<Entity> entities, IReactToGroupBatchedSystem system)
        {
            if(system is ISystemPreProcessor preProcessor)
            { preProcessor.BeforeProcessing(); }
            
            system.Process(EntityComponentAccessor, entities);
            
            if(system is ISystemPostProcessor postProcessor)
            { postProcessor.AfterProcessing(); }
        }
        
        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            { _systemSubscriptions.RemoveAndDispose(system); }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _systemSubscriptions.Values.DisposeAll();
                _systemSubscriptions.Clear();
            }
        }
    }
}