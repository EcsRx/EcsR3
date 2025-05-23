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
        { testKeyedCollection.TryGetValue(i, out _); }
        
        for(var i=0;i<Amount;i++)
        { testKeyedCollection.Remove(i); }
    }
    
    [Benchmark]
    public void AddLookupRemove_Hashset()
    {
        var testHashset = new HashSet<int>();
        
        for(var i=0;i<Amount;i++)
        { testHashset.Add(i); }
        
        for(var i=0;i<Amount;i++)
        { testHashset.TryGetValue(i, out _); }
        
        for(var i=0;i<Amount;i++)
        { testHashset.Remove(i); }
    }
}