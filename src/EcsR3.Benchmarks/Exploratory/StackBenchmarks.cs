using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EcsR3.Benchmarks.Exploratory;

[SimpleJob(RuntimeMoniker.Net90, warmupCount: 1, invocationCount: 1, iterationCount: 1)]
[MemoryDiagnoser]
public class StackBenchmarks
{
    [Params(1, 10000, 1000000)]
    public int ResizeAmount;
    
    public Stack<int> Stack;
    public IEnumerable<int> NewIndexes;

    [GlobalSetup]
    public void Setup()
    {
        Stack = new Stack<int>(Enumerable.Range(0, ResizeAmount));
        NewIndexes = Enumerable.Range(ResizeAmount+1, ResizeAmount*2);
    }
    
    [Benchmark]
    public void AddToStackIndividually_Enumerable()
    {
        foreach(var index in NewIndexes)
        { Stack.Push(index); }
    }

    [Benchmark]
    public void AddToStackIndividually_Array()
    {
        var newIndexes = NewIndexes.ToArray();
        for (var i = 0; i < newIndexes.Length; i++)
        { Stack.Push(newIndexes[i]); }
    }
    
    [Benchmark]
    public void AddToStackIndividually_Span()
    {
        var newIndexes = NewIndexes.ToArray().AsSpan();
        for (var i = 0; i < newIndexes.Length; i++)
        { Stack.Push(newIndexes[i]); }
    }
    
    [Benchmark]
    public void AddToStackInBatch_Enumerable()
    { Stack = new Stack<int>(Stack.Union(NewIndexes)); }
    
    [Benchmark]
    public void AddToStackInBatch_Array()
    { Stack = new Stack<int>(Stack.Union(NewIndexes).ToArray()); }

    [Benchmark]
    public void AddToStackInBatch_Span()
    {
        var stack = Stack.ToArray();
        var newIndexes = NewIndexes.ToArray().AsSpan();
        Array.Resize(ref stack, stack.Length + newIndexes.Length);
        
        Stack = new Stack<int>(stack);
    }
}