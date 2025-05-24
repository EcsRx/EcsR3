using SystemsR3.Threading;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Examples.ExampleApps.Performance.Components;
using EcsR3.Plugins.Batching.Systems;
using R3;

namespace EcsR3.Examples.ExampleApps.Performance.Systems
{
    public class ExampleBatchedSystem : ReferenceBatchedSystem<SimpleReadComponent, SimpleWriteComponent>
    {
        public ExampleBatchedSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
        {
        }

        protected override Observable<bool> ReactWhen()
        { return Observable.Never<bool>(); }
        
        protected override void Process(int EntityId, SimpleReadComponent component1, SimpleWriteComponent component2)
        {}
    }
}