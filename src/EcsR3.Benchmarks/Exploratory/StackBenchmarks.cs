using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 3)]
[MemoryDiagnoser]
public class StackBenchmarks
{
    [Params(1, 10000, 1000000)]
    public int ResizeAmount;
    
    public IEnumerable<int> NewIndexes;

    [GlobalSetup]
    public void Setup()
    { NewIndexes = Enumerable.Range(0, ResizeAmount); }
    
    [Benchmark]
    public void AddToStackIndividually_Enumerable()
    {
        var stack = new Stack<int>();
        foreach(var index in NewIndexes)
        { stack.Push(index); }
    }

    [Benchmark]
    public void AddToStackIndividually_Array()
    {
        var stack = new Stack<int>();
        var newIndexes = NewIndexes.ToArray();
        for (var i = 0; i < newIndexes.Length; i++)
        { stack.Push(newIndexes[i]); }
    }

    [Benchmark]
    public void AddToStackInBatch_Enumerable()
    {
        var stack = new Stack<int>();
        stack = new Stack<int>(stack.Union(NewIndexes));
    }

    [Benchmark]
    public void AddToStackInBatch_Array()
    {
        var stack = new Stack<int>();
        stack = new Stack<int>(stack.Union(NewIndexes).ToArray());
    }
}