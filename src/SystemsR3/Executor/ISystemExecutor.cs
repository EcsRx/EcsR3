using System.Collections.Generic;
using SystemsR3.Systems;

namespace SystemsR3.Executor
{
    public interface ISystemExecutor
    {
        IEnumerable<ISystem> Systems { get; }

        bool HasSystem(ISystem system);
        void RemoveSystem(ISystem system);
        void AddSystem(ISystem system);
    }
}