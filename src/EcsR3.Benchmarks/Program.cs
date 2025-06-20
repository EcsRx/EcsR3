﻿using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using EcsR3.Benchmarks.Benchmarks;
using EcsR3.Benchmarks.Exploratory;
using SystemsR3.Extensions;

namespace EcsR3.Benchmarks
{
    class Program
    {
        /// <summary>
        /// These benchmarks are mainly pointless and just noise at the moment due to not
        /// really understanding the best way to structure them without state bleed between tests
        /// GlobalSetup is not enough IterationSetup is too much, so unsure what to do with them in the long run.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var benchmarks = new []
            {
                
                BenchmarkConverter.TypeToBenchmarks(typeof(IdPoolBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(MultithreadedIdPoolBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(OptimizedIdPoolBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityRetrievalBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(EntityAddComponentsBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(MultipleComputedEntityGroupsAddAndRemoveBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ComputedEntityGroupsAddAndRemoveBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ComputedEntityGroupsAddAndRemoveWithNoiseBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ExecutorAddAndRemoveEntitySystemBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(BatchSystemMultiThreadingBenchmark)),
                
                BenchmarkConverter.TypeToBenchmarks(typeof(StackBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ArrayResizeBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(KeyedCollectionVsDictionaryBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(ParallelReadOnlyBenchmark)),
                BenchmarkConverter.TypeToBenchmarks(typeof(MultiDimensionalArrayResizeBenchmarks)),
                BenchmarkConverter.TypeToBenchmarks(typeof(IntValueLookupBenchmarks))
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