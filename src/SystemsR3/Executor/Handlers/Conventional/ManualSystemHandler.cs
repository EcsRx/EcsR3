using System;
using SystemsR3.Attributes;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;

namespace SystemsR3.Executor.Handlers.Conventional
{
    [Priority(100)]
    public class ManualSystemHandler : IConventionalSystemHandler
    {
        public bool CanHandleSystem(ISystem system)
        { return system is IManualSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            castSystem.StartSystem();
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            castSystem.StopSystem();
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}