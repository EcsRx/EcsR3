using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace EcsR3.Benchmarks.Exploratory;

public struct TestStruct(int x, int y)
{
    public int X = x;
    public int Y = y;
}

public class ParallelBenchmark
{
    private TestStruct[] array = null;
    private Memory<TestStruct> memory = null;

    [GlobalSetup]
    public void Setup()
    {
        array = new TestStruct[100_000];
        memory = array;
    }

    [Benchmark]
    public void ParallelArray()
    {
        Parallel.For(0, array.Length, i =>
        {
            var item = array[i];
            item.X = i;
            item.Y = i * 2;
            array[i] = item;
        });
    }

    [Benchmark]
    public void ParallelArrayPartitioner()
    {
        Parallel.ForEach(Partitioner.Create(0, array.Length), item =>
        {
            for (int i = item.Item1; i < item.Item2; i++)
            {
                var arrayItem = array[i];
                arrayItem.X = i;
                arrayItem.Y = i * 2;
                array[i] = arrayItem;
            }
        });
    }

    [Benchmark]
    public void ParallelMemory()
    {
        Parallel.For(0, memory.Length, i =>
        {
            var item = memory.Span[i];
            item.X = i;
            item.Y = i * 2;
            memory.Span[i] = item;
        });
    }

    [Benchmark]
    public void ParallelMemoryPartitioner()
    {
        Parallel.ForEach(Partitioner.Create(0, memory.Length), item =>
        {
            var span = memory.Span;
            for (int i = item.Item1; i < item.Item2; i++)
            {
                var memoryItem = span[i];
                memoryItem.X = i;
                memoryItem.Y = i * 2;
                span[i] = memoryItem;
            }
        });
    }
    
    [Benchmark]
    public void ParallelMemoryPartitionerPreSliced()
    {
        Parallel.ForEach(Partitioner.Create(0, memory.Length), item =>
        {
            var span = memory[item.Item1..item.Item2].Span;
            var index = 0;
            for (int i = item.Item1; i < item.Item2; i++)
            {
                var memoryItem = span[index];
                memoryItem.X = i;
                memoryItem.Y = i * 2;
                span[index] = memoryItem;
                index++;
            }
        });
    }
}