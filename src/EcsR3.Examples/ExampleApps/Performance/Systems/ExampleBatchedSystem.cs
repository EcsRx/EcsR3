using SystemsR3.Threading;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.Performance.Components;
using EcsR3.Systems.Batching;
using EcsR3.Systems.Batching.Convention;
using R3;

namespace EcsR3.Examples.ExampleApps.Performance.Systems
{
    public class ExampleBatchedSystem : BatchedSystem<SimpleReadComponent, SimpleWriteComponent>
    {
        public ExampleBatchedSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
        {
        }

        protected override Observable<Unit> ReactWhen()
        { return Observable.Never<Unit>(); }
        
        protected override void Process(Entity entity, SimpleReadComponent component1, SimpleWriteComponent component2)
        {}
    }
}