using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Systems.Reactive;
using SystemsR3.Attributes;
using SystemsR3.Executor.Handlers;
using SystemsR3.Extensions;
using SystemsR3.Systems;
using R3;

namespace EcsR3.Systems.Handlers
{
    [Priority(3)]
    public class ReactToDataSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityComponentAccessor EntityComponentAccessor;
        public readonly IComputedEntityGroupRegistry ComputedEntityGroupRegistry;
        public readonly IDictionary<ISystem, IDisposable> SystemSubscriptions;
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> EntitySubscriptions;

        private readonly MethodInfo _processEntityMethod;
        private readonly object _lock = new object();
        
        public ReactToDataSystemHandler(IEntityComponentAccessor entityComponentAccessor, IComputedEntityGroupRegistry computedEntityGroupRegistry)
        {
            EntityComponentAccessor = entityComponentAccessor;
            ComputedEntityGroupRegistry = computedEntityGroupRegistry;
            SystemSubscriptions = new Dictionary<ISystem, IDisposable>();
            EntitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
            _processEntityMethod = GetType().GetMethod("ProcessEntity");
        }

        // TODO: This is REALLY bad but currently no other way around the dynamic invocation lookup stuff
        public IEnumerable<Func<Entity, IDisposable>> CreateProcessorFunctions(ISystem system)
        {
            var genericMethods = system
                .GetGenericDataTypes(typeof(IReactToDataSystem<>))
                .Select(x => _processEntityMethod.MakeGenericMethod(x));

            foreach (var genericMethod in genericMethods)
            { yield return entity => (IDisposable) genericMethod.Invoke(this, new object[] {system, entity}); }
        }
        
        public IDisposable ProcessEntity<T>(IReactToDataSystem<T> system, Entity entity)
        {
            var reactObservable = system.ReactToData(EntityComponentAccessor, entity);
            return reactObservable.Subscribe(x => system.Process(EntityComponentAccessor, entity, x));
        }
        
        public bool CanHandleSystem(ISystem system)
        { return system.IsReactiveDataSystem(); }

        public void SetupSystem(ISystem system)
        {
            var processEntityFunctions = CreateProcessorFunctions(system).ToArray();

            var entityChangeSubscriptions = new CompositeDisposable();
            var entitySubscriptions = new Dictionary<int, IDisposable>();

            lock (_lock)
            {
                SystemSubscriptions.Add(system, entityChangeSubscriptions);
                EntitySubscriptions.Add(system, entitySubscriptions);
            }

            var groupSystem = system as IGroupSystem;
            var observableGroup = ComputedEntityGroupRegistry.GetComputedGroup(groupSystem.Group);

            observableGroup.OnAdded
                .Subscribe(x =>
                {
                    // This occurs if we have an add elsewhere removing the entity before this one is called
                    if (observableGroup.Contains(x))
                    { SetupEntity(processEntityFunctions, x, entitySubscriptions); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnRemoved
                .Subscribe(x =>
                {
                    if (entitySubscriptions.ContainsKey(x.Id)) 
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in observableGroup)
            { SetupEntity(processEntityFunctions, entity, entitySubscriptions); }
        }
        
        public void SetupEntity(IEnumerable<Func<Entity, IDisposable>> systemProcessors, Entity entity, Dictionary<int, IDisposable> subs)
        {
            lock(_lock)
            { subs.Add(entity.Id, null); }
                
            foreach (var processFunction in systemProcessors)
            {
                var subscription = processFunction(entity);

                lock (_lock)
                {
                    if (subs.ContainsKey(entity.Id))
                    { subs[entity.Id] = subscription; }
                    else
                    { subscription.Dispose(); }
                }
            }
        }

        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            {
                SystemSubscriptions.RemoveAndDispose(system);
            
                var entitySubscriptions = EntitySubscriptions[system];
                entitySubscriptions.Values.DisposeAll();
                entitySubscriptions.Clear();
                EntitySubscriptions.Remove(system);
            }
        }
        
        public void Dispose()
        {
            lock (_lock)
            {
                SystemSubscriptions.DisposeAll();
                foreach (var entitySubscriptions in EntitySubscriptions.Values)
                {
                    entitySubscriptions.Values.DisposeAll();
                    entitySubscriptions.Clear();
                }
                EntitySubscriptions.Clear();
            }
        }
    }
}