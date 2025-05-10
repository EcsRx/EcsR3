using System;
using System.Collections.Generic;
using System.Linq;
using SystemsR3.Attributes;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;

namespace SystemsR3.Extensions
{
    public static class ISystemExtensions
    {
        public static readonly Type ISystemType = typeof(ISystem);
        public static readonly Type IReactiveSystem = typeof(IReactiveSystem<>);
        public static readonly Type IReactToEventSystem = typeof(IReactToEventSystem<>);
        
        public static bool ShouldMutliThread(this ISystem system)
        {
            return system.GetType()
                       .GetCustomAttributes(typeof(MultiThreadAttribute), true)
                       .FirstOrDefault() != null;
        }
        
        public static IEnumerable<Type> GetGenericInterfacesFor(this ISystem system, Type systemType)
        {
            return system.GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == systemType);
        }
        
        public static bool MatchesSystemTypeWithGeneric(this ISystem system, Type systemType)
        { return GetGenericInterfacesFor(system, systemType).Any(); }
        
        public static IEnumerable<Type> GetGenericDataTypes(this ISystem system, Type systemType)
        {
            var matchingInterface = GetGenericInterfaceType(system, systemType);
            return matchingInterface.Select(x => x.GetGenericArguments()[0]);
        }

        public static IEnumerable<Type> GetGenericInterfaceType(this ISystem system, Type systemType)
        { return system.GetType().GetMatchingInterfaceGenericTypes(systemType); }
        
        public static bool IsReactToEventSystem(this ISystem system)
        { return system.MatchesSystemTypeWithGeneric(IReactToEventSystem); }
        
        public static bool IsReactiveSystem(this ISystem system)
        { return system.MatchesSystemTypeWithGeneric(IReactiveSystem); }
        
        public static IEnumerable<Type> GetSystemTypesImplemented(this ISystem system)
        {
            return system.GetType()
                .GetInterfaces()
                .Where(x => x.GetInterfaces()
                    .Any(y => y.IsAssignableFrom(ISystemType)));
        }
    }
}