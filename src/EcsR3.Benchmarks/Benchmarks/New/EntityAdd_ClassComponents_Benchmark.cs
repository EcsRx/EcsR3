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

        [Params(1, 2 ,3)]
        public int ComponentCount;
        
        [Params(true, false)]
        public bool PreAllocate;

        public override void Setup()
        {
            if (!PreAllocate) { return; }
            GetPoolFor<ClassComponent1>().Expand(EntityCount);
            if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Expand(EntityCount); }
            if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Expand(EntityCount); }
        }

        public override void Cleanup()
        {
            EntityCollection.RemoveAllEntities();
            GetPoolFor<ClassComponent1>().Clear();
            if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Clear(); }
            if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Clear(); }
        }

        [Benchmark]
        public void EntitiesBatchAdd_ClassComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.CreateEntity();
                switch (ComponentCount)
                {
                    case 1: entity.AddComponents(new ClassComponent1()); break;
                    case 2: entity.AddComponents(new ClassComponent1(), new ClassComponent2()); break;
                    case 3: entity.AddComponents(new ClassComponent1(), new ClassComponent2(), new ClassComponent3()); break;
                }
            }
        }
        
        [Benchmark]
        public void EntitiesAddIndividual_ClassComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.CreateEntity();
                entity.AddComponent<ClassComponent1>();
                if(ComponentCount >= 2) { entity.AddComponent<ClassComponent2>(); }
                if(ComponentCount >= 3) { entity.AddComponent<ClassComponent3>(); }
            }
        }
    }
}