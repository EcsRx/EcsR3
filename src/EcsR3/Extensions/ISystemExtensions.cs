using System;
using EcsR3.Groups;
using EcsR3.Systems;
using SystemsR3.Extensions;
using SystemsR3.Systems;

namespace EcsR3.Extensions
{
    public static class ISystemExtensions
    {
        public static IGroup GroupFor(this IGroupSystem groupSystem, params Type[] requiredTypes)
        { return new Group(requiredTypes); }
        
        public static bool IsSystemReactive(this ISystem system)
        { return system is IReactToEntitySystem || system is IReactToGroupSystem || system.IsReactiveDataSystem(); }

        public static bool IsReactiveDataSystem(this ISystem system)
        { return system.MatchesSystemTypeWithGeneric(typeof(IReactToDataSystem<>)); }
    }
}