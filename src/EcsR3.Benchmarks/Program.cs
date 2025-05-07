using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using EcsR3.Benchmarks.Benchmarks;
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
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityAddComponentsBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityGroupMatchingBenchmark)),*/
                BenchmarkConverter.TypeToBenchmarks(typeof(MultipleObservableGroupsAddAndRemoveBenchmark)),/*
                BenchmarkConverter.TypeToBenchmarks(typeof(ObservableGroupsAddAndRemoveBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ObservableGroupsAddAndRemoveWithNoiseBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ExecutorAddAndRemoveEntitySystemBenchmark)),*/
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