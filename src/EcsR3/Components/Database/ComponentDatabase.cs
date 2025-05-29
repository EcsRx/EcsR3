using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Components.Lookups;
using SystemsR3.Pools.Config;

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

        public bool HasPoolConfigFor(Type type)
        { return ComponentDatabaseConfig.PoolSpecificConfig.ContainsKey(type); }

        public PoolConfig GetPoolConfigFor(Type type)
        {
            return ComponentDatabaseConfig.PoolSpecificConfig
                .TryGetValue(type, out var config) ? config 
                : new PoolConfig(ComponentDatabaseConfig.InitialSize, ComponentDatabaseConfig.ExpansionSize, ComponentDatabaseConfig.MaxSize);
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
                    var componentType = componentTypes[i].Key;
                    if (ComponentDatabaseConfig.OnlyPreAllocatePoolsWithConfig && !HasPoolConfigFor(componentType))
                    {
                        ComponentData[i] = CreatePoolFor(componentType, new PoolConfig(0, ComponentDatabaseConfig.ExpansionSize, ComponentDatabaseConfig.MaxSize));
                        continue;
                    }
                    
                    var poolConfig = GetPoolConfigFor(componentType);
                    ComponentData[i] = CreatePoolFor(componentType, poolConfig);
                }     
            }
        }
        
        public IComponentPool<T> GetPoolFor<T>(int componentTypeId) where T : IComponent
        {
            lock (_lock)
            { return (IComponentPool<T>) ComponentData[componentTypeId]; }
        }
        
        public IComponentPool GetPoolFor(int componentTypeId)
        { return ComponentData[componentTypeId]; }
        
        public IComponentPool<T> GetPoolFor<T>() where T : IComponent
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(typeof(T));
            return GetPoolFor<T>(componentTypeId);
        }

        public IComponentPool GetPoolFor(Type type)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentTypeId(type);
            return ComponentData[componentTypeId];
        }

        public T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { return componentPool.Components[allocationIndex]; }
        }
        
        public IComponent Get(int componentTypeId, int allocationIndex)
        {
            var componentPool = GetPoolFor(componentTypeId);
            lock (_lock)
            { return componentPool.Get(allocationIndex); }
        }
        
        public T[] Get<T>(int componentTypeId, int[] allocationIndexes) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            var components = new T[allocationIndexes.Length];
            lock (_lock)
            {
                for (var i = 0; i < allocationIndexes.Length; i++)
                {
                    var allocationIndex = allocationIndexes[i];
                    components[i] = componentPool.Components[allocationIndex];
                }
            }
            return components;
        }

        public ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { return ref componentPool.Components[allocationIndex]; }
        }
        
        /*
        public ref T[] GetRef<T>(int componentTypeId, int[] allocationIndexes) where T : IComponent
        {
            var componentRefs = new ref T[allocationIndexes.Length];
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            {
                for (var i = 0; i < allocationIndexes.Length; i++)
                {
                    var allocationIndex = allocationIndexes[i];
                    componentRefs[i] = ref componentPool.Components[allocationIndex];
                }
            }
            return componentRefs;
        }*/

        public void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { componentPool.Components[allocationIndex] = component; }
        }

        public void Set<T>(int componentTypeId, int[] allocationIndexes, IReadOnlyList<T> components) where T : IComponent
        {
            var componentPool = GetPoolFor<T>(componentTypeId);
            lock (_lock)
            { componentPool.Set(allocationIndexes, components); }
        }

        public void SetMany(int[] componentTypeIds, int[] allocationIndexes, IReadOnlyList<IComponent> components)
        {
            if (componentTypeIds.Length != allocationIndexes.Length || componentTypeIds.Length != components.Count)
            { throw new ArgumentException("Component type ids, allocation indexs and components must all be the same length"); }
            
            lock (_lock)
            {
                for (var i = 0; i < components.Count; i++)
                {
                    var componentTypeId = componentTypeIds[i];
                    var allocationIndex = allocationIndexes[i];
                    var component = components[i];
                    ComponentData[componentTypeId].Set(allocationIndex, component);
                }
            }
        }

        public void Remove(int componentTypeId, int allocationIndex)
        {
            lock (_lock)
            { ComponentData[componentTypeId].Release(allocationIndex); }
        }
        
        public void Remove(int componentTypeId, int[] allocationIndex)
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
        
        public int[] Allocate(int componentTypeId, int count)
        {
            lock (_lock)
            {
                var pool = ComponentData[componentTypeId];
                return pool.Allocate(count);
            }
        }

        public void PreAllocateComponents(int componentTypeId, int? allocationSize = null)
        {
            lock (_lock)
            {
                var poolSpecificConfig = GetPoolConfigFor(ComponentTypeLookup.GetComponentType(componentTypeId));
                var pool = ComponentData[componentTypeId];
                pool.Expand(allocationSize ?? poolSpecificConfig.ExpansionSize);
            }
        }
    }
}