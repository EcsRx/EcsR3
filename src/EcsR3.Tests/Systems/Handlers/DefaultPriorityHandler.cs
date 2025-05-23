using System;
using SystemsR3.Executor.Handlers;
using SystemsR3.Systems;

namespace EcsR3.Tests.Systems.Handlers
{
    public class DefaultPriorityHandler : IConventionalSystemHandler
    {
        private Action _doSomethingOnSetup;
        private Action _doSomethingOnDestroy;

        public DefaultPriorityHandler(Action doSomethingOnSetup = null, Action doSomethingOnDestroy = null)
        {
            _doSomethingOnSetup = doSomethingOnSetup;
            _doSomethingOnDestroy = doSomethingOnDestroy;
        }

        public void Dispose() {}
        public bool CanHandleSystem(ISystem system) => true;

        public void SetupSystem(ISystem system)
        { _doSomethingOnSetup?.Invoke(); }

        public void DestroySystem(ISystem system)
        { _doSomethingOnDestroy?.Invoke(); }
    }
}