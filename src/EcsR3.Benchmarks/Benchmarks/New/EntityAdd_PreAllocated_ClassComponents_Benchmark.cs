using BenchmarkDotNet.Attributes;
using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Extensions;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks.Benchmarks.New
{
    [BenchmarkCategory("Entities")]
    public class EntityAdd_PreAllocated_ClassComponents_Benchmark : EcsR3Benchmark
    {
        private IEntity[] _entities;
        
        [Params(100000)]
        public int EntityCount;

        [Params(1, 2 ,3)]
        public int ComponentCount;

        public override void Setup()
        {
            _entities = new IEntity[EntityCount];
            GetPoolFor<ClassComponent1>().Expand(EntityCount);
            if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Expand(EntityCount); }
            if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Expand(EntityCount); }
        }

        public override void Cleanup()
        {
            _entities.ForEachRun(x => x.RemoveAllComponents());
            GetPoolFor<ClassComponent1>().Clear();
            if(ComponentCount >= 2) { GetPoolFor<ClassComponent2>().Clear(); }
            if(ComponentCount == 3) { GetPoolFor<ClassComponent3>().Clear(); }
        }

        [Benchmark]
        public void EntitiesBatchAdd_PreAllocated_ClassComponents()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                _entities[i] = new Entity(i, ComponentDatabase, ComponentTypeLookup, EntityChangeRouter);
                switch (ComponentCount)
                {
                    case 1: _entities[i].AddComponents(new ClassComponent1()); break;
                    case 2: _entities[i].AddComponents(new ClassComponent1(), new ClassComponent2()); break;
                    case 3: _entities[i].AddComponents(new ClassComponent1(), new ClassComponent2(), new ClassComponent3()); break;
                }
            }
        }
        
        [Benchmark]
        public void EntitiesAddIndividual_PreAllocated_ClassComponents()
        {
            for (var i = 0; i < _entities.Length; i++)
            {
                _entities[i] = new Entity(i, ComponentDatabase, ComponentTypeLookup, EntityChangeRouter);
                _entities[i].AddComponent<ClassComponent1>();
                if(ComponentCount >= 2) { _entities[i].AddComponent<ClassComponent2>(); }
                if(ComponentCount >= 3) { _entities[i].AddComponent<ClassComponent3>(); }
            }
        }
    }
}