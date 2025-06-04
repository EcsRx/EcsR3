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
    public class ReactToGroupSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityComponentAccessor EntityComponentAccessor;
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;       
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        private readonly object _lock = new object();
        
        public ReactToGroupSystemHandler(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroupRegistry computedEntityGroupRegistry, IThreadHandler threadHandler)
        {
            EntityComponentAccessor = entityComponentAccessor;
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            _threadHandler = threadHandler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToGroupSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToGroupSystem)system;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);
            var reactObservable = castSystem.ReactToGroup(observableGroup);
            var runParallel = system.ShouldMutliThread();
                
            IDisposable noPredicateSub;

            noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.ToArray(), castSystem, runParallel));

            lock (_lock)
            { _systemSubscriptions.Add(system, noPredicateSub); }
        }

        private void ExecuteForGroup(IReadOnlyList<Entity> entities, IReactToGroupSystem system, bool runParallel = false)
        {
            if(system is ISystemPreProcessor preProcessor)
            { preProcessor.BeforeProcessing(); }
            
            if (runParallel)
            {
                _threadHandler.For(0, entities.Count, i =>
                { system.Process(EntityComponentAccessor, entities[i]); });
                return;
            }
            
            for (var i = entities.Count - 1; i >= 0; i--)
            { system.Process(EntityComponentAccessor, entities[i]); }
            
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
