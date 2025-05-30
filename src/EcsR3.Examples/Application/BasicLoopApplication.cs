using System;
using System.Diagnostics;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Examples.Application
{
    public abstract class BasicLoopApplication : EcsR3ConsoleApplication
    {
        protected static readonly int EntityCount = 200000;
        protected static readonly int SimulatedUpdates = 100;
        
        protected override void ApplicationStarted()
        {
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
                SetupEntity(EntityComponentAccessor, entity);              
            }
        }

        protected abstract string Description { get; }
        protected abstract void SetupEntity(IEntityComponentAccessor entityComponentAccessor, int entityId);
        protected abstract void RunProcess();
    }
}