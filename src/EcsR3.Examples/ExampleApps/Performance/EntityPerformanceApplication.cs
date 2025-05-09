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
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.Performance
{
    public class EntityPerformanceApplication : EcsR3ConsoleApplication
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private static readonly int EntityCount = 100000;

        private List<IEntity> _entities;

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
            EntityCollection.RemoveAllEntities();
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
            
            if(entity.HasAllComponents(_availableComponentTypes))
            { ignore = true; }
            
            var component1 = entity.GetComponent<ClassComponent1>();
            var component2 = entity.GetComponent<ClassComponent2>();
            var component3 = entity.GetComponent<ClassComponent3>();
            var component4 = entity.GetComponent<ClassComponent4>();
            var component5 = entity.GetComponent<ClassComponent5>();
            var component6 = entity.GetComponent<ClassComponent6>();
            var component7 = entity.GetComponent<ClassComponent7>();
            var component8 = entity.GetComponent<ClassComponent8>();
            var component9 = entity.GetComponent<ClassComponent9>();
            var component10 = entity.GetComponent<ClassComponent10>();
            var component11 = entity.GetComponent<ClassComponent11>();
            var component12 = entity.GetComponent<ClassComponent12>();
            var component13 = entity.GetComponent<ClassComponent13>();
            var component14 = entity.GetComponent<ClassComponent14>();
            var component15 = entity.GetComponent<ClassComponent15>();
            var component16 = entity.GetComponent<ClassComponent16>();
            var component17 = entity.GetComponent<ClassComponent17>();
            var component18 = entity.GetComponent<ClassComponent18>();
            var component19 = entity.GetComponent<ClassComponent19>();
            var component20 = entity.GetComponent<ClassComponent20>();

            // Stop optimizing away the usages
            if(component1 == null) { }
            if(component2 == null) { }
            if(component3 == null) { }
            if(component4 == null) { }
            if(component5 == null) { }
            if(component6 == null) { }
            if(component7 == null) { }
            if(component8 == null) { }
            if(component9 == null) { }
            if(component10 == null) { }
            if(component11 == null) { }
            if(component12 == null) { }
            if(component13 == null) { }
            if(component14 == null) { }
            if(component15 == null) { }
            if(component16 == null) { }
            if(component17 == null) { }
            if(component18 == null) { }
            if(component19 == null) { }
            if(component20 == null) { }
        }
    }
}