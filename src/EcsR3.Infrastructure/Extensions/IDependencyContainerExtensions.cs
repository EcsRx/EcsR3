using System;
using SystemsR3.Infrastructure.Dependencies;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Collections;
using EcsR3.Computeds;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Registries;
using EcsR3.Groups;

namespace EcsR3.Infrastructure.Extensions
{
    public static class IDependencyContainerExtensions
    {
        /// <summary>
        /// Resolves an observable group
        /// </summary>
        /// <param name="resolver">The container to action on</param>
        /// <param name="group">The group to observe</param>
        /// <returns>The observable group</returns>
        public static IComputedEntityGroup ResolveComputedGroup(this IDependencyResolver resolver, IGroup group)
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
        public static IComputedEntityGroup ResolveComputedGroup(this IDependencyResolver resolver, params Type[] componentTypes)
        {
            var collectionManager = resolver.Resolve<IComputedEntityGroupRegistry>();
            var group = new Group(componentTypes);
            return collectionManager.GetComputedGroup(group);
        }
    }
}