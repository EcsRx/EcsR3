using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 1)]
[MemoryDiagnoser]
public class ArrayResizeBenchmarks
{
    [Params(100,10000,100000)]
    public int ResizeAmount;
    
    public int[] TestArray;

    [GlobalSetup]
    public void Setup()
    {
        TestArray = new int[ResizeAmount];
    }
    
    [Benchmark]
    public void ResizeArray_BuiltIn()
    {
        Array.Resize(ref TestArray, TestArray.Length + ResizeAmount);
    }
    
    [Benchmark]
    public void ResizeArray_Handrolled()
    {
        var temp = new int[TestArray.Length + ResizeAmount];            
        TestArray.CopyTo(temp, 0);
        TestArray = temp;
    }

}