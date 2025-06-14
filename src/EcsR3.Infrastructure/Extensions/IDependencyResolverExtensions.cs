using System;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Components;
using EcsR3.Computeds.Components;
using EcsR3.Computeds.Components.Registries;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Groups;

namespace EcsR3.Infrastructure.Extensions
{
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Resolves an observable group
        /// </summary>
        /// <param name="resolver">The container to action on</param>
        /// <param name="group">The group to observe</param>
        /// <returns>The observable group</returns>
        public static IComputedEntityGroup ResolveComputedEntityGroup(this IDependencyResolver resolver, IGroup group)
        {
            var collectionManager = resolver.Resolve<IComputedEntityGroupRegistry>();
            return collectionManager.GetComputedGroup(group);
        }
        
        /// <summary>
        /// Resolves an observable group
        /// </summary>
        /// <param name="resolver">The container to action on</param>
        /// <param name="componentTypes">The required components for the group to observe</param>
        /// <returns></returns>
        public static IComputedEntityGroup ResolveComputedEntityGroup(this IDependencyResolver resolver, params Type[] componentTypes)
        {
            var collectionManager = resolver.Resolve<IComputedEntityGroupRegistry>();
            var group = new Group(componentTypes);
            return collectionManager.GetComputedGroup(group);
        }
        
        public static IComputedComponentGroup<T1> ResolveComputedComponentGroup<T1>(this IDependencyResolver resolver) where T1 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1>();
        }
        
        public static IComputedComponentGroup<T1, T2> ResolveComputedComponentGroup<T1, T2>(this IDependencyResolver resolver) where T1 : IComponent where T2 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1, T2>();
        }
        
        public static IComputedComponentGroup<T1, T2, T3> ResolveComputedComponentGroup<T1, T2, T3>(this IDependencyResolver resolver) where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1, T2, T3>();
        }
        
        public static IComputedComponentGroup<T1, T2, T3, T4> ResolveComputedComponentGroup<T1, T2, T3, T4>(this IDependencyResolver resolver) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4>();
        }
                
        public static IComputedComponentGroup<T1, T2, T3, T4, T5> ResolveComputedComponentGroup<T1, T2, T3, T4, T5>(this IDependencyResolver resolver) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5>();
        }
                        
        public static IComputedComponentGroup<T1, T2, T3, T4, T5, T6> ResolveComputedComponentGroup<T1, T2, T3, T4, T5, T6>(this IDependencyResolver resolver) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5, T6>();
        }
        
        public static IComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7> ResolveComputedComponentGroup<T1, T2, T3, T4, T5, T6, T7>(this IDependencyResolver resolver) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent where T6 : IComponent where T7 : IComponent
        {
            var computedComponentGroupRegistry = resolver.Resolve<IComputedComponentGroupRegistry>();
            return computedComponentGroupRegistry.GetComputedGroup<T1, T2, T3, T4, T5, T6, T7>();
        }
    }
}