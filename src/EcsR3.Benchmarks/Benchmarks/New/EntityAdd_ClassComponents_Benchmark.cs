using BenchmarkDotNet.Attributes;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks.New
{
    [BenchmarkCategory("Entities")]
    public class EntityAdd_ClassComponents_Benchmark : EcsR3Benchmark
    {
        [Params(100000)]
        public int EntityCount;

        [Params(1, 2 ,3, 10)]
        public int ComponentCount;
        
        [Params(true, false)]
        public bool PreAllocate;

        [IterationSetup]
        public void IterationSetup()
        {
            if (PreAllocate)
            {
                GetPoolFor<ClassComponent1>().Expand(EntityCount);
                if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Expand(EntityCount); }
                if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Expand(EntityCount); }
            }
        }

        [IterationCleanup]
        public override void Cleanup()
        {
            EntityCollection.RemoveAll();
            GetPoolFor<ClassComponent1>().Clear();
            if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Clear(); }
            if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Clear(); }
        }

        [Benchmark]
        public void EntitiesBatchAdd_ClassComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entityId = EntityCollection.Create();
                switch (ComponentCount)
                {
                    case 1: EntityComponentAccessor.CreateComponent<ClassComponent1>(entityId); break;
                    case 2: EntityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2>(entityId); break;
                    case 3: EntityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2, ClassComponent3>(entityId); break;
                }
            }
        }
        
        [Benchmark]
        public void EntitiesAddIndividual_ClassComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entityId = EntityCollection.Create();
                EntityComponentAccessor.CreateComponent<ClassComponent1>(entityId);
                if(ComponentCount >= 2) { EntityComponentAccessor.CreateComponent<ClassComponent2>(entityId); }
                if(ComponentCount >= 3) { EntityComponentAccessor.CreateComponent<ClassComponent3>(entityId); }
            }
        }
    }
}