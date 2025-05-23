using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SystemsR3.Threading;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Playground.Components;
using EcsR3.Plugins.Batching.Batches;

namespace EcsR3.Examples.ExampleApps.Playground.StructBased
{
    public class Struct4BApplication : BasicLoopApplication
    {
        private PinnedBatch<StructComponent, StructComponent2> _componentBatch;
        private readonly IThreadHandler ThreadHandler = new DefaultThreadHandler();
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            
            var batchBuilder = _batchBuilderFactory.Create<StructComponent, StructComponent2>();
            _componentBatch = batchBuilder.Build(EntityCollection.ToArray());
        }

        protected override string Description { get; } = "Uses auto batching to group components mixed with multithreading";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            unsafe
            {
                var size = _componentBatch.Batches.Length;
                ThreadHandler.For(0, size, i =>
                {
                    ref var batch = ref _componentBatch.Batches[i];
                    ref var basic = ref *batch.Component1;
                    ref var basic2 = ref *batch.Component2;
                    basic.Position += Vector3.One;
                    basic.Something += 10;
                    basic2.IsTrue = 1;
                    basic2.Value += 10;
                });
            }
        }
    }
}