using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Components;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.Performance.Components.Class;
using EcsR3.Examples.ExampleApps.Performance.Helper;
using EcsR3.Examples.ExampleApps.Performance.Modules;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class OptimizedEntityPerformanceApplication : EcsR3ConsoleApplication
    {
        private IComponent[] _availableComponents;
        private int[] _availableComponentTypeIds;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private static readonly int EntityCount = 100000;

        private List<IEntity> _entities;

        protected override void LoadModules()
        { DependencyRegistry.LoadModule<OptimizedEcsRxInfrastructureModule>(); }
        
        protected override void BindSystems()
        {}

        protected override void ApplicationStarted()
        {                       
            var componentNamespace = typeof(ClassComponent1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            _availableComponentTypeIds = Enumerable.Range(0, 20).ToArray();
            
            var componentDatabase = DependencyResolver.Resolve<IComponentDatabase>();
            var componentTypeLookup = DependencyResolver.Resolve<IComponentTypeLookup>();
            var entityChangeRouter = DependencyResolver.Resolve<IEntityChangeRouter>();
                        
            _entities = new List<IEntity>();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, componentDatabase, componentTypeLookup, entityChangeRouter);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);                
            }

            var timeTaken = ProcessEntities();

            Console.WriteLine($"Finished In: {timeTaken.Milliseconds}ms");
        }

        private TimeSpan ProcessEntities()
        {
            EntityCollection.RemoveAll();
            GC.Collect();
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < EntityCount; i++)
            { ProcessEntity(_entities[i]); }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
        
        public void ProcessEntity(IEntity entity)
        {
            // Called just to make sure the method runs
            bool ignore;
            
            if(entity.HasAllComponents(_availableComponentTypeIds))
            { ignore = true; }

            for (var i = 0; i < _availableComponentTypeIds.Length; i++)
            {
                var component = entity.GetComponent(i);
                if(component == null) { }
            }
        }
    }
}