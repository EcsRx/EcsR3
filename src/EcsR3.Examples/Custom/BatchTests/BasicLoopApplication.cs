using System;
using System.Diagnostics;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.BatchTests.Components;
using SystemsR3.Infrastructure.Extensions;

namespace EcsR3.Examples.Custom.BatchTests
{
    public abstract class BasicLoopApplication : EcsR3ConsoleApplication
    {
        protected static readonly int EntityCount = 200000;
        protected static readonly int SimulatedUpdates = 100;
        protected IComponentTypeLookup _componentTypeLookup;

        protected int ClassComponent1TypeId;
        protected int ClassComponent2TypeId;
        protected int StructComponent1TypeId;
        protected int StructComponent2TypeId;
        
        protected override void ApplicationStarted()
        {
            _componentTypeLookup = DependencyResolver.Resolve<IComponentTypeLookup>();

            ClassComponent1TypeId = _componentTypeLookup.GetComponentTypeId(typeof(ClassComponent));
            ClassComponent2TypeId = _componentTypeLookup.GetComponentTypeId(typeof(ClassComponent2));
            StructComponent1TypeId = _componentTypeLookup.GetComponentTypeId(typeof(StructComponent));
            StructComponent2TypeId = _componentTypeLookup.GetComponentTypeId(typeof(StructComponent2));
            
            var name = GetType().Name;
            Console.WriteLine($"{name} - {Description}");
            var timer = Stopwatch.StartNew();
            SetupEntities();
            timer.Stop();
            Console.WriteLine($"{name} - Setting up {EntityCount} entities in {timer.ElapsedMilliseconds}ms");
            
            timer.Reset();
            timer.Start();
            for(var update=0;update<SimulatedUpdates;update++)
            { RunProcess(); }
            timer.Stop();
            var totalProcessTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"{name} - Simulating {SimulatedUpdates} updates - Processing {EntityCount} entities in {totalProcessTime.TotalMilliseconds}ms");
            Console.WriteLine();
        }

        protected virtual void SetupEntities()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = EntityCollection.Create();
                SetupEntity(entity);              
            }
        }

        protected abstract string Description { get; }
        protected abstract void SetupEntity(IEntity entity);
        protected abstract void RunProcess();
    }
}