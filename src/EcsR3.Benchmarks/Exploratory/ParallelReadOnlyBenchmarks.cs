using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace EcsR3.Benchmarks.Exploratory;

public readonly struct TestStruct2(int value)
{
    public readonly int Value = value;
}

public class ParallelReadOnlyBenchmark
{
    private TestStruct2[] array = null;
    private ReadOnlyMemory<TestStruct2> memoryReadOnly = null;

    [GlobalSetup]
    public void Setup()
    {
        array = new TestStruct2[100_000];
        for (var i = 0; i < array.Length; i++)
        { array[i] = new TestStruct2(i); }
        
        memoryReadOnly = array;
    }

    [Benchmark]
    public void ParallelArray()
    {
        long total = 0;
        Parallel.For(0, array.Length, i =>
        {
            var item = array[i];
            total += item.Value;
        });
    }

    [Benchmark]
    public void ParallelArrayPartitioner()
    {
        long total = 0;
        Parallel.ForEach(Partitioner.Create(0, array.Length), item =>
        {
            for (int i = item.Item1; i < item.Item2; i++)
            {
                var arrayItem = array[i];
                total += arrayItem.Value;
            }
        });
    }

    [Benchmark]
    public void ParallelMemory()
    {
        long total = 0;
        Parallel.For(0, memoryReadOnly.Length, i =>
        {
            var item = memoryReadOnly.Span[i];
            total += item.Value;
        });
    }

    [Benchmark]
    public void ParallelMemoryPartitioner()
    {
        long total = 0;
        Parallel.ForEach(Partitioner.Create(0, memoryReadOnly.Length), item =>
        {
            var span = memoryReadOnly.Span;
            for (int i = item.Item1; i < item.Item2; i++)
            {
                var memoryItem = span[i];
                total = memoryItem.Value;
            }
        });
    }
    
    [Benchmark]
    public void ParallelMemoryPartitionerPreSliced()
    {
        long total = 0;
        Parallel.ForEach(Partitioner.Create(0, memoryReadOnly.Length), item =>
        {
            var span = memoryReadOnly[item.Item1..item.Item2].Span;
            var index = 0;
            for (int i = item.Item1; i < item.Item2; i++)
            {
                var memoryItem = span[index++];
                total += memoryItem.Value;
            }
        });
    }
}