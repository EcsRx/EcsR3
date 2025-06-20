using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class ToArrayWithLogicBenchmarks
{
    [Params(100,10000,100000)]
    public int InputSize;
    
    [Params(100,10000,100000)]
    public int OutputSize;

    public bool ShouldKeep(int value)
    { return value % 2 == 0; }
    
    [Benchmark]
    public IReadOnlyList<int> ToArray_Linq()
    {
        var baseData = Enumerable.Range(0, InputSize).ToArray();
        return baseData.Where(ShouldKeep).Take(OutputSize).ToArray();
    }
    
    [Benchmark]
    public int[] ToArray_Manual()
    {
        var baseData = Enumerable.Range(0, InputSize).ToArray();
        var result = new int[OutputSize];
        var indexUsed = 0;
        for (var i = 0; i < baseData.Length; i++)
        {
            var value = baseData[i];
            if(ShouldKeep(value))
            { result[indexUsed++] = value; }
            
            if(indexUsed == OutputSize) { break; }
        }
        return result;
    }
    
    [Benchmark]
    public int[] ToArray_SpanOversize()
    {
        var baseData = Enumerable.Range(0, InputSize).ToArray();
        Span<int> result = new int[InputSize];
        var indexUsed = 0;
        for (var i = 0; i < baseData.Length; i++)
        {
            var value = baseData[i];
            if(ShouldKeep(value))
            { result[indexUsed++] = value; }
            
            if(indexUsed == OutputSize) { break; }
        }
        return result[..indexUsed].ToArray();
    }
        
    [Benchmark]
    public int[] ToArray_Span()
    {
        var baseData = Enumerable.Range(0, InputSize).ToArray();
        Span<int> result = new int[OutputSize];
        var indexUsed = 0;
        for (var i = 0; i < baseData.Length; i++)
        {
            var value = baseData[i];
            if(ShouldKeep(value))
            { result[indexUsed++] = value; }
            
            if(indexUsed == OutputSize) { break; }
        }
        return result.ToArray();
    }
            
    [Benchmark]
    public int[] ToArray_List()
    {
        var baseData = Enumerable.Range(0, InputSize).ToArray();
        var list = new List<int>();
        for (var i = 0; i < baseData.Length; i++)
        {
            var value = baseData[i];
            if(ShouldKeep(value))
            { list.Add(value); }
            
            if(list.Count == OutputSize) { break; }
        }
        return list.ToArray();
    }
}