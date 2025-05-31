using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EcsR3.Computeds.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Augments;
using EcsR3.Systems.Reactive;
using R3;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class IsPatternMatchingBenchmark
{
    class SomeClassWithType : IReactToGroupSystem, ISystemPreProcessor, ISystemPostProcessor
    {
        public IGroup Group { get; }
        public void BeforeProcessing(){}
        public void AfterProcessing(){}
        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup) { return null; }
        public void Process(IEntityComponentAccessor entityComponentAccessor, int entityId){}
    }
    
    class SomeClassWithoutType : IReactToGroupSystem
    {
        public IGroup Group { get; }
        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup) { return null; }
        public void Process(IEntityComponentAccessor entityComponentAccessor, int entityId){}
    }
    
    [Params(10000,100000,10000000)]
    public int TimesToCheck;
    
    [Benchmark]
    public void ProcessEachWithoutCheck()
    {
        var instances = new SomeClassWithType[TimesToCheck];
        for(int i = 0; i < TimesToCheck; i++)
        { instances[i] = new SomeClassWithType(); }
        
        for (int i = 0; i < instances.Length; i++)
        {
            var instance = instances[i];
            instance.Process(null, 0);
        }
    }
    
    [Benchmark]
    public void ProcessEachWithCheckAndTypes()
    {
        var instances = new SomeClassWithType[TimesToCheck];
        for(int i = 0; i < TimesToCheck; i++)
        { instances[i] = new SomeClassWithType(); }
        
        for (int i = 0; i < instances.Length; i++)
        {
            var instance = instances[i];
            
            instance.Process(null, 0);
            
            if(instance is ISystemPreProcessor preProcessor)
            { preProcessor.BeforeProcessing(); }
            
            if(instance is ISystemPreProcessor postProcessor)
            { postProcessor.BeforeProcessing(); }
        }
    }
    
        
    [Benchmark]
    public void ProcessEachWithCheckButWithoutTypes()
    {
        var instances = new SomeClassWithoutType[TimesToCheck];
        for(int i = 0; i < TimesToCheck; i++)
        { instances[i] = new SomeClassWithoutType(); }
        
        for (int i = 0; i < instances.Length; i++)
        {
            var instance = instances[i];
            
            instance.Process(null, 0);
            
            if(instance is ISystemPreProcessor preProcessor)
            { preProcessor.BeforeProcessing(); }
            
            if(instance is ISystemPreProcessor postProcessor)
            { postProcessor.BeforeProcessing(); }
        }
    }
}