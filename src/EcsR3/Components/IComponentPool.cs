using System;
using System.Collections;
using System.Collections.Generic;
using R3;

namespace EcsR3.Components
{
    /// <summary>
    /// Acts as a basic memory block for components of a specific type
    /// </summary>
    /// <remarks>This helps speed up access when you want components of the same type</remarks>
    /// <typeparam name="T">Type of component</typeparam>
    public interface IComponentPool<T> : IComponentPool, IDisposable
        where T : IComponent
    {
        /// <summary>
        /// Contiguous block of memory for components 
        /// </summary>
        T[] Components { get; }
        
        /// <summary>
        /// Set a component to a specific index
        /// </summary>
        /// <param name="index">the index to set</param>
        /// <param name="value">the component to use</param>
        void Set(int index, T value);
        
        /// <summary>
        /// Set a component to a specific index
        /// </summary>
        /// <param name="index">the indexes to set</param>
        /// <param name="value">the components to use</param>
        void Set(int[] index, T[] value);
    }
    
    /// <summary>
    /// Acts as a basic memory block for components of a specific type
    /// </summary>
    /// <remarks>This helps speed up access for components of the same type</remarks>
    public interface IComponentPool : IEnumerable
    {
        /// <summary>
        /// Amount of components within the pool
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// The type of component this pool contains
        /// </summary>
        Type ComponentType { get; }
        
        /// <summary>
        /// The amount of indexes remaining in this pool before a resize is needed
        /// </summary>
        int IndexesRemaining { get; }
        
        /// <summary>
        /// To notify when a pool has extended
        /// </summary>
        Observable<bool> OnPoolExtending { get; }
        
        /// <summary>
        /// Expand the pool automatically or by a given amount
        /// </summary>
        /// <param name="amountToAdd"></param>
        void Expand(int? amountToAdd = null);
        
        /// <summary>
        /// Set a component to a specific index
        /// </summary>
        /// <param name="index">the index to set</param>
        /// <param name="value">the component to use</param>
        void Set(int index, IComponent value);

        /// <summary>
        /// Returns the component at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IComponent Get(int index);
        
        /// <summary>
        /// Allocates space for the pools and indexes
        /// </summary>
        /// <returns>The id of the allocation</returns>
        int Allocate();
        
        /// <summary>
        /// Allocates multiple spaces for the pools and indexes
        /// </summary>
        /// <returns>The ids of the allocations</returns>
        int[] Allocate(int count);
        
        /// <summary>
        /// Releases the component at the given index
        /// </summary>
        /// <param name="index"></param>
        void Release(int index);
        
        /// <summary>
        /// Releases the components at the given indexes
        /// </summary>
        /// <param name="indexes"></param>
        void Release(int[] indexes);

        /// <summary>
        /// Used to manually clear the pool of all contents
        /// </summary>
        void Clear();
    }
}