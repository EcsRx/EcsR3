using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

public class TestKeyedCollection : KeyedCollection<int, int>
{
    protected override int GetKeyForItem(int item)
    {
        return item;
    }
}

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class KeyedCollectionVsDictionaryBenchmarks
{
    [Params(100,10000,100000)]
    public int Amount;
    
    public TestKeyedCollection TestKeyedCollection = new TestKeyedCollection();
    public Dictionary<int,int> TestDictionary = new Dictionary<int,int>();

    [GlobalSetup]
    public void Setup()
    {
        TestKeyedCollection.Clear();
        TestDictionary.Clear();
    }
   
    [Benchmark]
    public void AddLookupRemove_Dictionary()
    {
        var testDictionary = new Dictionary<int,int>();
        
        for(var i=0;i<Amount;i++)
        { testDictionary.Add(i,i); }
        
        for(var i=0;i<Amount;i++)
        { testDictionary.TryGetValue(i, out _); }
        
        for(var i=0;i<Amount;i++)
        { testDictionary.Remove(i); }
    }
    
    [Benchmark]
    public void AddLookupRemove_Lookup()
    {
        var testKeyedCollection = new TestKeyedCollection();
        
        for(var i=0;i<Amount;i++)
        { testKeyedCollection.Add(i); }
        
        for(var i=0;i<Amount;i++)
        { TestKeyedCollection.TryGetValue(i, out _); }
        
        for(var i=0;i<Amount;i++)
        { TestKeyedCollection.Remove(i); }
    }
}