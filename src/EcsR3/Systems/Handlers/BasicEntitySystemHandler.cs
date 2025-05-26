using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Groups;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Extensions;
using SystemsR3.Scheduling;
using SystemsR3.Systems;
using SystemsR3.Threading;
using EcsR3.Extensions;
using R3;

namespace EcsR3.Systems.Handlers
{
    [Priority(6)]
    public class BasicEntitySystemHandler : IConventionalSystemHandler
    {
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        public readonly IUpdateScheduler UpdateScheduler;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        private readonly object _lock = new object();
        
        public BasicEntitySystemHandler(IComputedEntityGroupRegistry computedEntityGroupRegistry, IThreadHandler threadHandler, IUpdateScheduler updateScheduler)
        {
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            _threadHandler = threadHandler;
            UpdateScheduler = updateScheduler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IBasicEntitySystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IBasicEntitySystem)system;

            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(castSystem.Group);
            var hasEntityPredicate = castSystem.Group is IHasPredicate;
            var runParallel = system.ShouldMutliThread();
            IDisposable subscription;
            
            if (!hasEntityPredicate)
            {
                subscription = UpdateScheduler.OnUpdate
                    .Subscribe(x => ExecuteForGroup(observableGroup.ToArray(), castSystem, runParallel));
            }
            else
            {
                var groupPredicate = castSystem.Group as IHasPredicate;
                subscription = UpdateScheduler.OnUpdate
                    .Subscribe(x => ExecuteForGroup(observableGroup
                        .Where(groupPredicate.CanProcessEntity).ToList(), castSystem, runParallel));
            }

            lock (_lock)
            { _systemSubscriptions.Add(system, subscription); }
        }

        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IBasicEntitySystem castSystem, bool runParallel = false)
        {
            var elapsedTime = UpdateScheduler.ElapsedTime;
            if (runParallel)
            {
                _threadHandler.For(0, entities.Count, i =>
                { castSystem.Process(entities[i], elapsedTime); });
                return;
            }
            
            for (var i = entities.Count - 1; i >= 0; i--)
            { castSystem.Process(entities[i], elapsedTime); }
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