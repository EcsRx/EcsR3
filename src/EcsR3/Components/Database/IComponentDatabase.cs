using System;

namespace EcsR3.Components.Database
{
    public interface IComponentDatabase : IBatchComponentDatabase
    {
        T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent;
        IComponent Get(int componentTypeId, int allocationIndex);
        ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent;
        void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent;
        void Remove(int componentTypeId, int allocationIndex);
        int Allocate(int componentTypeId);
        
        void PreAllocateComponents(int componentTypeId, int? allocationSize = null);
        
        IComponentPool<T> GetPoolFor<T>(int componentTypeId) where T : IComponent;
        IComponentPool<T> GetPoolFor<T>() where T : IComponent;
        IComponentPool GetPoolFor(int componentTypeId);
        IComponentPool GetPoolFor(Type type);
    }
}