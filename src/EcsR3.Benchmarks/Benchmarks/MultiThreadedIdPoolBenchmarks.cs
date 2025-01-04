using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using SystemsR3.Pools;

namespace EcsR3.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Pools")]
    public class MultithreadedIdPoolBenchmarks : EcsR3Benchmark
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
        public void MultithreadedAllocationAndRelease()
        {
            Parallel.For(0, PoolCount, i => IdList[i] = IdPool.AllocateInstance());
            Parallel.For(0, PoolCount, i => IdPool.ReleaseInstance( IdList[i]));
        }
    }
}