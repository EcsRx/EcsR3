using System;
using System.Collections.Generic;
using SystemsR3.Extensions;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Scheduling;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;

namespace SystemsR3.Executor.Handlers.Conventional
{
    [Priority(6)]
    public class BasicSystemHandler : IConventionalSystemHandler
    {
        public readonly IUpdateScheduler UpdateScheduler;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public BasicSystemHandler(IUpdateScheduler updateScheduler)
        {
            UpdateScheduler = updateScheduler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IBasicSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IBasicSystem)system;
            var subscription = UpdateScheduler.OnUpdate.Subscribe(x => castSystem.Execute(x));
            _systemSubscriptions.Add(system, subscription);
        }
        
        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}