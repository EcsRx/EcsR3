using BenchmarkDotNet.Attributes;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Components.Struct;
using EcsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks.New
{
    [BenchmarkCategory("Entities")]
    public class EntityAdd_StructComponents_PreAllocatedBenchmark : EcsR3Benchmark
    {
        [Params(100000)]
        public int EntityCount;

        [Params(1, 2, 3)]
        public int ComponentCount;
        
        [IterationSetup]
        public override void Cleanup()
        {
            EntityCollection.Clear();
            GetPoolFor<StructComponent1>().Clear();
            if(ComponentCount >= 2) { GetPoolFor<StructComponent2>().Clear(); }
            if(ComponentCount == 3) { GetPoolFor<StructComponent3>().Clear(); }
        }

        [Benchmark]
        public void EntitiesBatchAdd_StructComponents_PreAllocated()
        {
            EntityAllocationDatabase.PreAllocate(EntityCount);
            GetPoolFor<StructComponent1>().Expand(EntityCount);
            if(ComponentCount >= 2) { GetPoolFor<StructComponent2>().Expand(EntityCount); }
            if(ComponentCount == 3) { GetPoolFor<StructComponent3>().Expand(EntityCount); }

            var entities = EntityCollection.CreateMany(EntityCount);
            
            switch (ComponentCount)
            {
                case 1: EntityComponentAccessor.CreateComponent<StructComponent1>(entities); break;
                case 2: EntityComponentAccessor.CreateComponents<StructComponent1, StructComponent2>(entities); break;
                case 3: EntityComponentAccessor.CreateComponents<StructComponent1, StructComponent2, StructComponent3>(entities); break;
            }
            
        }
        
        [Benchmark]
        public void EntitiesAddIndividual_StructComponents_PreAllocated()
        {
            EntityAllocationDatabase.PreAllocate(EntityCount);
            GetPoolFor<StructComponent1>().Expand(EntityCount);
            if(ComponentCount >= 2) { GetPoolFor<StructComponent2>().Expand(EntityCount); }
            if(ComponentCount == 3) { GetPoolFor<StructComponent3>().Expand(EntityCount); }
            
            for (var i = 0; i < EntityCount; i++)
            {
                var entityId = EntityCollection.Create();
                EntityComponentAccessor.CreateComponent<StructComponent1>(entityId);
                if(ComponentCount >= 2) { EntityComponentAccessor.CreateComponent<StructComponent2>(entityId); }
                if(ComponentCount >= 3) { EntityComponentAccessor.CreateComponent<StructComponent3>(entityId); }
            }
        }
    }
}