using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class ArrayResizeBenchmarks
{
    [Params(100,10000,100000)]
    public int ResizeAmount;
    
    [Benchmark]
    public int[] ResizeArray_BuiltIn()
    {
        var array = new int[ResizeAmount];
        Array.Resize(ref array, array.Length + ResizeAmount);
        return array;
    }
    
    [Benchmark]
    public int[] ResizeArray_Handrolled()
    {
        var array = new int[ResizeAmount];
        var temp = new int[array.Length + ResizeAmount];            
        array.CopyTo(temp, 0);
        array = temp;
        return array;
    }
}