using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Extensions;
using SystemsR3.Scheduling;
using SystemsR3.Systems;
using SystemsR3.Threading;
using EcsR3.Systems.Augments;
using R3;

namespace EcsR3.Systems.Handlers
{
    [Priority(6)]
    public class BasicEntitySystemHandler : IConventionalSystemHandler
    {
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        public readonly IUpdateScheduler UpdateScheduler;
        public readonly IEntityComponentAccessor EntityComponentAccessor;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        private readonly object _lock = new object();
        
        public BasicEntitySystemHandler(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroupRegistry computedEntityGroupRegistry, IThreadHandler threadHandler, IUpdateScheduler updateScheduler)
        {
            EntityComponentAccessor = entityComponentAccessor;
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
            var runParallel = system.ShouldMutliThread();
            IDisposable subscription;
            
            subscription = UpdateScheduler.OnUpdate
                .Subscribe(x => ExecuteForGroup(observableGroup.ToArray(), castSystem, runParallel));
           
            lock (_lock)
            { _systemSubscriptions.Add(system, subscription); }
        }

        private void ExecuteForGroup(IReadOnlyList<Entity> entities, IBasicEntitySystem castSystem, bool runParallel = false)
        {
            if(castSystem is ISystemPreProcessor preProcessor)
            { preProcessor.BeforeProcessing(); }
            
            var elapsedTime = UpdateScheduler.ElapsedTime;
            if (runParallel)
            {
                _threadHandler.For(0, entities.Count, i =>
                { castSystem.Process(EntityComponentAccessor, entities[i], elapsedTime); });
                return;
            }
            
            for (var i = entities.Count - 1; i >= 0; i--)
            { castSystem.Process(EntityComponentAccessor, entities[i], elapsedTime); }
            
            if(castSystem is ISystemPostProcessor postProcessor)
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