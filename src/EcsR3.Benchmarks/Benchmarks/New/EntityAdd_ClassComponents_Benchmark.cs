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
                    case 1: EntityComponentAccessor.AddComponents(entityId, new ClassComponent1()); break;
                    case 2: EntityComponentAccessor.AddComponents(entityId, new ClassComponent1(), new ClassComponent2()); break;
                    case 3: EntityComponentAccessor.AddComponents(entityId, new ClassComponent1(), new ClassComponent2(), new ClassComponent3()); break;
                }
            }
        }
        
        [Benchmark]
        public void EntitiesAddIndividual_ClassComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entityId = EntityCollection.Create();
                EntityComponentAccessor.AddComponent<ClassComponent1>(entityId);
                if(ComponentCount >= 2) { EntityComponentAccessor.AddComponent<ClassComponent2>(entityId); }
                if(ComponentCount >= 3) { EntityComponentAccessor.AddComponent<ClassComponent3>(entityId); }
            }
        }
    }
}