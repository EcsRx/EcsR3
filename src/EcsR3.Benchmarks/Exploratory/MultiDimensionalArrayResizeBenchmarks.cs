using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EcsR3.Helpers;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class MultiDimensionalArrayResizeBenchmarks
{
    [Params(100,1000, 5000)]
    public int xInflation;
    
    [Params(100,1000, 5000)]
    public int yInflation;
    
    [Benchmark(OperationsPerInvoke = 2048)]
    public void ResizeArray()
    {
        var array = new int[xInflation/2,yInflation/2];
        ArrayHelper.Resize2DArray(ref array, xInflation, yInflation);
    }    
    
    [Benchmark(OperationsPerInvoke = 2048)]
    public void ResizeArray_NoSpan()
    {
        var array = new int[xInflation/2,yInflation/2];
        ArrayHelper.Resize2DArray_NoSpan(ref array, xInflation, yInflation);
    }
    
    [Benchmark(OperationsPerInvoke = 2048)]
    public void ResizeArray_WithDefaults()
    {
        var array = new int[xInflation/2,yInflation/2];
        ArrayHelper.Resize2DArray(ref array, xInflation, yInflation, -1);
    }    
    
    [Benchmark(OperationsPerInvoke = 2048)]
    public void ResizeArray_WithDefaults_NoSpan()
    {
        var array = new int[xInflation/2,yInflation/2];
        ArrayHelper.Resize2DArray_NoSpan(ref array, xInflation, yInflation, -1);
    }
}