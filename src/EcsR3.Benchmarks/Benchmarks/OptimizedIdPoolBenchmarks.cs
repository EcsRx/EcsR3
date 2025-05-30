using BenchmarkDotNet.Attributes;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Pools")]
    public class OptimizedIdPoolBenchmarks : EcsR3Benchmark
    {
        [Params(1000, 10000, 100000)]
        public int PoolCount;

        public IIdPool IdPool { get; set; }
        public int[] IdList;

        public override void Setup()
        {
            var poolConfig = new PoolConfig(PoolCount);
            IdPool = new IdPool(poolConfig);
            IdList = new int[PoolCount];
        }

        public override void Cleanup()
        { }

        [Benchmark]
        public void OptimizedAllocationAndRelease()
        {
            for (var i = 0; i < PoolCount; i++)
            { IdList[i] = IdPool.Allocate(); }
            
            for(var i = 0; i < PoolCount; i++)
            { IdPool.Release(IdList[i]); }
        }
    }
}