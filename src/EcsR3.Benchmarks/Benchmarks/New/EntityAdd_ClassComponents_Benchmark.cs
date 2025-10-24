using BenchmarkDotNet.Attributes;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks.New
{
    [BenchmarkCategory("Entities")]
    public class EntityAdd_ClassComponents_Benchmark : EcsR3Benchmark
    {
        [Params(10000)]
        public int EntityCount;

        [Params(1, 2, 3)]
        public int ComponentCount;
        
        [IterationSetup]
        public override void Setup()
        {
            EntityCollection.Clear();
            GetPoolFor<ClassComponent1>().Clear();
            if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Clear(); }
            if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Clear(); }
        }

        [Benchmark]
        public void EntitiesBatchAdd_ClassComponents()
        {
            var entities = EntityCollection.CreateMany(EntityCount);
            switch (ComponentCount)
            {
                case 1: EntityComponentAccessor.CreateComponent<ClassComponent1>(entities); break;
                case 2: EntityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2>(entities); break;
                case 3: EntityComponentAccessor.CreateComponents<ClassComponent1, ClassComponent2, ClassComponent3>(entities); break;
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