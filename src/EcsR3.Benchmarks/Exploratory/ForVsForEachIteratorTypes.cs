using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class ForVsForEachIteratorTypes
{
    [Params(100,10000,100000)]
    public int ElementCount;
    
    [Params(1000)]
    public int Iterations;
    
    [Benchmark]
    public int[] For_Array()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            for (var i = 0; i < ElementCount; i++)
            {
                index++;
                arrayOut[index] = arrayIn[index];
            }
        }
        return arrayOut;
    }
    
    [Benchmark]
    public int[] For_IReadOnlyList()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        IReadOnlyList<int> customIn = arrayIn;
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            for (var i = 0; i < ElementCount; i++)
            {
                index++;
                arrayOut[index] = customIn[index];
            }
        }
        return arrayOut;
    }    
        
    [Benchmark]
    public int[] For_Span()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        Span<int> customIn = arrayIn;
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            for (var i = 0; i < ElementCount; i++)
            {
                index++;
                arrayOut[index] = customIn[index];
            }
        }
        return arrayOut;
    } 
    
        
    [Benchmark]
    public int[] ForEach_Array()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            foreach (var value in arrayIn)
            {
                index++;
                arrayOut[index] = value;
            }
        }
        return arrayOut;
    }
    
    [Benchmark]
    public int[] ForEach_ReadOnlyCollection()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        IReadOnlyCollection<int> customIn = arrayIn;
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            foreach (var value in customIn)
            {
                index++;
                arrayOut[index] = value;
            }
        }
        return arrayOut;
    }
   
    
    [Benchmark]
    public int[] ForEach_ReadOnlyList()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        IReadOnlyList<int> customIn = arrayIn;
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            foreach (var value in customIn)
            {
                index++;
                arrayOut[index] = value;
            }
        }
        return arrayOut;
    }
    
    [Benchmark]
    public int[] ForEach_Span()
    {
        var arrayIn = new int[ElementCount];
        var arrayOut = new int[ElementCount];
        Span<int> customIn = arrayIn;
        
        for (var _ = 0; _ < Iterations; _++)
        {
            var index = -1;
            foreach (var value in customIn)
            {
                index++;
                arrayOut[index] = value;
            }
        }
        return arrayOut;
    }
}