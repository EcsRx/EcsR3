using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class MultipleArrayUpdateBenchmarks
{
    [Params(100,10000,100000)]
    public int ArraySize;
    
    [Params(5,10,100)]
    public int NumberOfArrays;
    
    [Benchmark]
    public int[][] IteratePerArray()
    {
        var arrays = new int[NumberOfArrays][];
        for (var i = 0; i < NumberOfArrays; i++)
        { arrays[i] = new int[ArraySize]; }

        for (var i = 0; i < NumberOfArrays; i++)
        {
            var array = arrays[i];
            for (var j = 0; j < ArraySize; j++)
            { array[j] = 10; }
        }
        return arrays;
    }
}