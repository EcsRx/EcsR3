﻿using BenchmarkDotNet.Attributes;
using SystemsR3.Pools;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Pools")]
    public class IdPoolBenchmarks : EcsR3Benchmark
    {
        [Params(1000, 10000, 100000)]
        public int PoolCount;

        public IIdPool IdPool { get; set; }
        public int[] IdList;

        public override void Setup()
        {
            IdPool = new IdPool();
            IdList = new int[PoolCount];
        }

        public override void Cleanup()
        { }

        [Benchmark]
        public void DefaultAllocationAndRelease()
        {
            for (var i = 0; i < PoolCount; i++)
            { IdList[i] = IdPool.Allocate(); }
            
            for(var i = 0; i < PoolCount; i++)
            { IdPool.Release(IdList[i]); }
        }
    }
}