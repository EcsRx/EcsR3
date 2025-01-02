using System;
using SystemsR3.Systems;

namespace SystemsR3.Executor.Handlers
{
    public interface IConventionalSystemHandler : IDisposable
    {
        bool CanHandleSystem(ISystem system);
        void SetupSystem(ISystem system);
        void DestroySystem(ISystem system);
    }
}