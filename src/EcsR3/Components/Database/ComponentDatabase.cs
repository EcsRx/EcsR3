using System;
using System.Linq;
using EcsR3.Components.Lookups;
using SystemsR3.Pools;

namespace EcsR3.Components.Database
{
    public class ComponentDatabase : IComponentDatabase
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public ComponentDatabaseConfig ComponentDatabaseConfig { get; }
        
        public IComponentPool[] ComponentData { get; private set; }
        
        private readonly object _lock = new object();

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, ComponentDatabaseConfig componentDatabaseConfig = null)
        {
            ComponentTypeLookup = componentTypeLookup;
            ComponentDatabaseConfig = componentDatabaseConfig ?? new ComponentDatabaseConfig();
            Initialize();
        }

        public IComponentPool CreatePoolFor(Type type, PoolConfig poolConfig = null)
        {
            var componentPoolType = typeof(ComponentPool<>);
            Type[] typeArgs = { type };
            var genericComponentPoolType = componentPoolType.MakeGenericType(typeArgs);
            return (IComponentPool)Activator.CreateInstance(genericComponentPoolType, poolConfig);
        }
        
        public void Initialize()
        {
            lock (_lock)
            {
                var componentTypes = ComponentTypeLookup.GetComponentTypeMappings().ToArray();
                var componentCount = componentTypes.Length;
                ComponentData = new IComponentPool[componentCount];

                for (var i = 0; i < componentCount; i++)
                {
                    var poolTuningConfig = ComponentDatabaseConfig.PoolSpecificConfig
                        .TryGetValue(componentTypes[i].Key, out var config) 
                        ? config 
                        : new PoolConfig(ComponentDatabaseConfig.InitialSize, ComponentDatabaseConfig.ExpansionSize, ComponentDatabaseConfig.MaxSize);
                    
                    ComponentData[i] = CreatePoolFor(componentTypes[i].Key, poolTuningConfig);
                }     
            }
        }

        public IComponentPool<T> GetPoolFor<T>(int componentTypeId) where T : IComponent
        {
            lock (_lock)
            { return (IComponentPool<T>) ComponentData[componentTypeId]; }
        }

        public T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { return componentPool.Components[allocationIndex]; }
        }

        public ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { return ref componentPool.Components[allocationIndex]; }
            
        }

        public T[] GetComponents<T>(int componentTypeId) where T : IComponent
        { return GetPoolFor<T>(componentTypeId).Components; }

        public void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { componentPool.Components[allocationIndex] = component; }
        }

        public void Remove(int componentTypeId, int allocationIndex)
        {
            lock (_lock)
            { ComponentData[componentTypeId].Release(allocationIndex); }
        }

        public int Allocate(int componentTypeId)
        {
            lock (_lock)
            {
                var pool = ComponentData[componentTypeId];
                return pool.Allocate();
            }
        }

        public void PreAllocateComponents(int componentTypeId, int? allocationSize = null)
        {
            lock (_lock)
            {
                var pool = ComponentData[componentTypeId];
                pool.Expand(allocationSize ?? ComponentDatabaseConfig.ExpansionSize);;
            }
        }
    }
}