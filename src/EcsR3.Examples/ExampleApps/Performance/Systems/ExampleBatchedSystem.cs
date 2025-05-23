using System;
using SystemsR3.Threading;
using EcsR3.Collections;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components;
using EcsR3.Plugins.Batching.Factories;
using EcsR3.Plugins.Batching.Systems;
using R3;

namespace EcsR3.Examples.ExampleApps.Performance.Systems
{
    public class ExampleBatchedSystem : ReferenceBatchedSystem<SimpleReadComponent, SimpleWriteComponent>
    {
        public ExampleBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, 
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IComputedGroupManager computedGroupManager)
            : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler, computedGroupManager)
        {}

        protected override Observable<bool> ReactWhen()
        { return Observable.Never<bool>(); }

        // This shows that every time the group changes, it should throttle (actually debounce) and run after 10ms
        protected override Observable<IEntity> ProcessGroupSubscription(Observable<IEntity> groupChange)
        { return groupChange.Debounce(TimeSpan.FromMilliseconds(10)); }

        protected override void Process(int EntityId, SimpleReadComponent component1, SimpleWriteComponent component2)
        {}
    }
}