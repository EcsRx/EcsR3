using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CommunityToolkit.HighPerformance;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class TwoDimensionalArrayTypeBenchmarks
{
    [Params(1, 10, 100)]
    public int XSize;
    
    [Params(1, 10000, 100000)]
    public int YSize;
    
    [GlobalSetup]
    public void Setup()
    {  }
    
    [Benchmark]
    public void SetValuesForAllIndexes_MultiDimensional()
    {
        var array = new int[XSize, YSize];
        var arraySpan = new Span2D<int>(array);
        arraySpan.Fill(1);
    }

    [Benchmark]
    public void SetValuesForAllIndexes_Jagged()
    {
        var array = new int[XSize][];
        for (var x = 0; x < array.Length; x++) 
        { 
            array[x] = new int[YSize];
            for (var y = 0; y < array[x].Length; y++)
            {
                array[x][y] = 1;
            } 
        }  
    }

    [Benchmark]
    public void GetValuesAlongX_MultiDimensional()
    {
        var yIndex = YSize / 2;
        var array = new int[XSize, YSize];
        var arraySpan = new ReadOnlySpan2D<int>(array);
        var results = arraySpan.GetColumn(yIndex).ToArray();
    }
    /*
    [Benchmark]
    public void GetValuesAlongX_Jagged()
    {
        var index = XSize / 2;
        var array = new int[XSize][];
        for (var x = 0; x < array.Length; x++) 
        { array[x] = new int[YSize]; }  
        
        var result = new int[YSize];
        for (var y = 0; y < YSize; y++)
        { result[y] = array[index][y]; }
    }*/
}