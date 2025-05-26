using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using EcsR3.Components.Database;
using EcsR3.Computeds.Components.Registries;
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
        
        [MultiThread]
        public class ClassBatchSystem : BatchedSystem<ClassComponent, ClassComponent2>
        {
            public ClassBatchSystem(IComponentDatabase componentDatabase, IComputedComponentGroupRegistry computedComponentGroupRegistry, IThreadHandler threadHandler) : base(componentDatabase, computedComponentGroupRegistry, threadHandler)
            {}

            protected override Observable<bool> ReactWhen()
            { return Observable.Never<bool>(); }

            public void ForceRun() => ProcessBatch();
            
            protected override void Process(int entityId, ClassComponent component1, ClassComponent2 component2)
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
            var entities = EntityCollection.CreateMany<BatchClassComponentBlueprint>(100000);
            var batchSystem = new ClassBatchSystem(ComponentDatabase, ComputedComponentGroupRegistry, ThreadHandler);
            batchSystem.StartSystem();
            GC.Collect();
            
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