using System;
using EcsR3.Examples.Custom;
using EcsR3.Examples.Custom.BatchTests;
using EcsR3.Examples.Custom.ComputedComponents;
using EcsR3.Examples.ExampleApps.BatchedGroupExample;
using EcsR3.Examples.ExampleApps.ComputedGroupExample;
using EcsR3.Examples.ExampleApps.DataPipelinesExample;
using EcsR3.Examples.ExampleApps.HealthExample;
using EcsR3.Examples.ExampleApps.HelloWorldExample;
using EcsR3.Examples.ExampleApps.LoadingEntityDatabase;
using EcsR3.Examples.ExampleApps.Performance;
using Spectre.Console;

namespace EcsR3.Examples
{
    class Program
    {
        class Example
        {
            public string Name { get; }
            public Action Executor { get; }

            public Example(string name, Action executor)
            {
                Name = name;
                Executor = executor;
            }
        }
        
        static void Main(string[] args)
        {
            var availableExamples = new []
            {
                new Example("Scenario: Hello World", () => new HelloWorldExampleApplication().StartApplication()),
                new Example("Scenario: Computed Groups", () => new ComputedGroupExampleApplication().StartApplication()),
                new Example("Scenario: Health Deduction", () => new HealthExampleApplication().StartApplication()),
                new Example("Scenario: Data Persistence", () => new PersistDataApplication().StartApplication()),
                new Example("Scenario: Entity Databases", () => new LoadingEntityDatabaseApplication().StartApplication()),
                new Example("Scenario: Batched Groups", () => new BatchedGroupExampleApplication().StartApplication()),
                new Example("Scenario: System Priorities", () => new SetupSystemPriorityApplication().StartApplication()),
                
                new Example("Performance: Systems", () => new SimpleSystemApplication().StartApplication()),
                new Example("Performance: Default Entity", () => new EntityPerformanceApplication().StartApplication()),
                new Example("Performance: Optimised Entity", () => new OptimizedEntityPerformanceApplication().StartApplication()),
                new Example("Performance: Default Group", () => new GroupPerformanceApplication().StartApplication()),
                new Example("Performance: Optimised Group", () => new OptimizedGroupPerformanceApplication().StartApplication()),
                new Example("Performance: Entity Creation", () => new MakingLotsOfEntitiesApplication().StartApplication()),
                
                //new Example("Dev: Manual Class Batching", () => new ManualClassBatchingApplication().StartApplication()),
                //new Example("Dev: Manual Struct Batching", () => new ManualStructBatchingApplication().StartApplication())
            };

            var exampleSelector = new SelectionPrompt<Example>()
                    .Title("Welcome To The [green]EcsR3[/] examples, what example do you want to run?")
                    .PageSize(50)
                    .MoreChoicesText("[grey](Move up and down to reveal more examples)[/]")
                    .UseConverter(x => x.Name)
                    .AddChoices(availableExamples);
            
            //var exampleToRun = AnsiConsole.Prompt(exampleSelector);
            //exampleToRun.Executor();
            
            new ClassBatchedSystemApplication().StartApplication();
        }
    }
}
