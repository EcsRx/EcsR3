using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using EcsR3.Benchmarks.Benchmarks.New;
using EcsR3.Benchmarks.Exploratory;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmarks = new []
            {
                /*
                BenchmarkConverter.TypeToBenchmarks(typeof(IdPoolBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(MultithreadedIdPoolBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(OptimizedIdPoolBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityRetrievalBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityAddComponentsBenchmark)),*/
                //BenchmarkConverter.TypeToBenchmarks(typeof(PreAllocated_EntityAdd_StructComponents_Benchmark)),
                /*
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityGroupMatchingBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(MultipleObservableGroupsAddAndRemoveBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ObservableGroupsAddAndRemoveBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ObservableGroupsAddAndRemoveWithNoiseBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ExecutorAddAndRemoveEntitySystemBenchmark))*/
                
                //BenchmarkConverter.TypeToBenchmarks(typeof(StackBenchmarks)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(ArrayResizeBenchmarks)),
                //BenchmarkConverter.TypeToBenchmarks(typeof(KeyedCollectionVsDictionaryBenchmarks))
                
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityAdd_ClassComponents_Benchmark)),
            };
            
            var summaries = BenchmarkRunner.Run(benchmarks);
            var consoleLogger = ConsoleLogger.Default;
            consoleLogger.Flush();
            summaries.ForEachRun(x =>
            {
                AsciiDocExporter.Default.ExportToLog(x, consoleLogger);
                consoleLogger.WriteLine();
            });
        }
    }
}