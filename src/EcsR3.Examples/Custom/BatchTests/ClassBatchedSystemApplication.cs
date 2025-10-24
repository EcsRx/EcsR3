using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
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
using SystemsR3.Attributes;
using SystemsR3.Infrastructure.Extensions;
using SystemsR3.Threading;

namespace EcsR3.Examples.Custom.BatchTests
{
    public class ClassBatchedSystemApplication : EcsR3ConsoleApplication
    {
        public IThreadHandler ThreadHandler { get; private set; }
        
        public class ClassBatchSystem : BatchedSystem<ClassComponent, ClassComponent2>
        {
            public ClassBatchSystem(IComponentDatabase componentDatabase, IEntityComponentAccessor entityComponentAccessor, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, entityComponentAccessor, computedComponentGroupRegistry, threadHandler)
            {
            }

            protected override Observable<Unit> ReactWhen()
            { return Observable.Never<Unit>(); }

            public void ForceRun() => ProcessBatch();
            
            protected override void Process(Entity entity, ClassComponent component1, ClassComponent2 component2)
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
            var batchSystem = new ClassBatchSystem(ComponentDatabase, EntityComponentAccessor, ComputedComponentGroupRegistry, ThreadHandler);
            batchSystem.StartSystem();
            var entities = EntityCollection.CreateMany<BatchClassComponentBlueprint>(EntityComponentAccessor, 100000);
            
            Console.WriteLine("Starting");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 100000; i++)
            { batchSystem.ForceRun(); }
            stopwatch.Stop();
            Console.WriteLine($"Time Taken: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}