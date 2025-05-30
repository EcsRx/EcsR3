using EcsR3.Components;
using SystemsR3.Utility;

namespace EcsR3.Entities.Accessors
{
    public interface IBatchEntityComponentAccessor
    {
        /// <summary>
        /// Creates the component for multiple entities at once
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponent<T>(int[] entityIds) where T : IComponent, new();
        
        /// <summary>
        /// Creates multiple components for multiple entities at once
        /// </summary>
        /// <typeparam name="T1">The type of the component</typeparam>
        /// <typeparam name="T2">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponents<T1, T2>(int[] entityIds) where T1 : IComponent, new() where T2 : IComponent, new();
        
        /// <summary>
        /// Creates multiple components for multiple entities at once
        /// </summary>
        /// <typeparam name="T1">The type of the component</typeparam>
        /// <typeparam name="T2">The type of the component</typeparam>
        /// <typeparam name="T3">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponents<T1, T2, T3>(int[] entityIds) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new();
                
        /// <summary>
        /// Creates multiple components for multiple entities at once
        /// </summary>
        /// <typeparam name="T1">The type of the component</typeparam>
        /// <typeparam name="T2">The type of the component</typeparam>
        /// <typeparam name="T3">The type of the component</typeparam>
        /// <typeparam name="T4">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponents<T1, T2, T3, T4>(int[] entityIds) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new();
        
        /// <summary>
        /// Creates multiple components for multiple entities at once
        /// </summary>
        /// <typeparam name="T1">The type of the component</typeparam>
        /// <typeparam name="T2">The type of the component</typeparam>
        /// <typeparam name="T3">The type of the component</typeparam>
        /// <typeparam name="T4">The type of the component</typeparam>
        /// <typeparam name="T5">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponents<T1, T2, T3, T4, T5>(int[] entityIds) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new();
        
        /// <summary>
        /// Creates multiple components for multiple entities at once
        /// </summary>
        /// <typeparam name="T1">The type of the component</typeparam>
        /// <typeparam name="T2">The type of the component</typeparam>
        /// <typeparam name="T3">The type of the component</typeparam>
        /// <typeparam name="T4">The type of the component</typeparam>
        /// <typeparam name="T5">The type of the component</typeparam>
        /// <typeparam name="T6">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponents<T1, T2, T3, T4, T5, T6>(int[] entityIds) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new();
        
        /// <summary>
        /// Creates multiple components for multiple entities at once
        /// </summary>
        /// <typeparam name="T1">The type of the component</typeparam>
        /// <typeparam name="T2">The type of the component</typeparam>
        /// <typeparam name="T3">The type of the component</typeparam>
        /// <typeparam name="T4">The type of the component</typeparam>
        /// <typeparam name="T5">The type of the component</typeparam>
        /// <typeparam name="T6">The type of the component</typeparam>
        /// <remarks>As you are batch creating you need to pull out the components via GetComponent if you want them</remarks>
        void CreateComponents<T1, T2, T3, T4, T5, T6, T7>(int[] entityIds) where T1 : IComponent, new() where T2 : IComponent, new() where T3 : IComponent, new() where T4 : IComponent, new() where T5 : IComponent, new() where T6 : IComponent, new() where T7 : IComponent, new();
        
        /// <summary>
        /// Gets the components for multiple entities at once
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        T[] GetComponent<T>(int[] entityIds) where T : IComponent, new();
        
        /// <summary>
        /// Gets the components for multiple entities at once
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        RefBuffer<T> GetComponentRef<T>(int[] entityIds) where T : struct, IComponent;
    }
}