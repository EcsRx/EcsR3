using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class IntValueLookupBenchmarks
{
    [Params(100,10000,100000)]
    public int Amount;
    
    [Benchmark]
    public bool[] Lookup_Array()
    {
        var accesses = new bool[Amount];
        var testArray = Enumerable.Range(0, Amount).ToArray();
        
        for(var i=0;i<Amount;i++)
        { accesses[i] = testArray.Contains(i); }

        return accesses;
    }
    
    [Benchmark]
    public bool[] Lookup_HashSet()
    {
        var accesses = new bool[Amount];
        var hashset = new HashSet<int>(Enumerable.Range(0, Amount));
        
        for(var i=0;i<Amount;i++)
        { accesses[i] = hashset.Contains(i); }

        return accesses;
    }
}