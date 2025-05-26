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
using SystemsR3.Threading;

namespace EcsR3.Systems.Handlers
{
    [Priority(6)]
    public class ReactToGroupSystemHandler : IConventionalSystemHandler
    {
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;       
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        private readonly object _lock = new object();
        
        public ReactToGroupSystemHandler(IComputedEntityGroupRegistry computedEntityGroupRegistry, IThreadHandler threadHandler)
        {
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            _threadHandler = threadHandler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToGroupExSystem || system is IReactToGroupSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToGroupSystem)system;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);
            var groupPredicate = castSystem.Group as IHasPredicate;
            var isExtendedSystem = system is IReactToGroupExSystem;
            var reactObservable = castSystem.ReactToGroup(observableGroup);
            var runParallel = system.ShouldMutliThread();
                
            if (groupPredicate == null)
            {
                IDisposable noPredicateSub;

                if (isExtendedSystem)
                { noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.ToArray(), (IReactToGroupExSystem)castSystem, runParallel)); }
                else
                { noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.ToArray(), castSystem, runParallel)); }

                lock (_lock)
                { _systemSubscriptions.Add(system, noPredicateSub); }
                
                return;
            }

            
            IDisposable predicateSub;
            
            if (isExtendedSystem)
            { predicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Where(groupPredicate.CanProcessEntity).ToArray(), (IReactToGroupExSystem)castSystem, runParallel)); }
            else
            { predicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Where(groupPredicate.CanProcessEntity).ToArray(), castSystem, runParallel)); }

            lock (_lock)
            { _systemSubscriptions.Add(system, predicateSub); }
        }

        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IReactToGroupSystem system, bool runParallel = false)
        {
            if (runParallel)
            {
                _threadHandler.For(0, entities.Count, i =>
                { system.Process(entities[i]); });
                return;
            }
            
            for (var i = entities.Count - 1; i >= 0; i--)
            { system.Process(entities[i]); }
        }
        
        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IReactToGroupExSystem system, bool runParallel = false)
        {
            // We manually down cast this so it doesnt recurse this method
            var castSystem = (IReactToGroupSystem)system;
            
            system.BeforeProcessing();
            ExecuteForGroup(entities, castSystem, runParallel);
            system.AfterProcessing();
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
