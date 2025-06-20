using System;
using System.Diagnostics;
using System.Numerics;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.BatchTests.Blueprints;
using EcsR3.Examples.Custom.BatchTests.Components;
using EcsR3.Extensions;
using EcsR3.Systems.Batching.Convention;
using R3;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Threading;

namespace EcsR3.Examples.Custom.BatchTests
{
    public class StructBatchedSystemApplication : EcsR3ConsoleApplication
    {
        public IThreadHandler ThreadHandler { get; private set; }
        
        public class StructBatchSystem : BatchedRefSystem<StructComponent, StructComponent2>
        {
            public StructBatchSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
            {
            }

            protected override Observable<Unit> ReactWhen()
            { return Observable.Never<Unit>(); }

            public void ForceRun() => ProcessBatch();
            
            protected override void Process(Entity entity, ref StructComponent component1, ref StructComponent2 component2)
            {
                component1.Position += Vector3.One;
                component1.Something += 10;
                component2.IsTrue = true;
                component2.Value += 10;
            }
        }

        protected override void ResolveApplicationDependencies()
        {
            base.ResolveApplicationDependencies();
            ThreadHandler = DependencyResolver.Resolve<IThreadHandler>();
        }

        protected override void ApplicationStarted()
        {
            EntityCollection.CreateMany<BatchStructComponentBlueprint>(EntityComponentAccessor, 100000);
            var batchSystem = new StructBatchSystem(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, ThreadHandler);
            batchSystem.StartSystem();
            
            Console.WriteLine("Starting");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 1000; i++)
            { batchSystem.ForceRun(); }
            stopwatch.Stop();
            Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}