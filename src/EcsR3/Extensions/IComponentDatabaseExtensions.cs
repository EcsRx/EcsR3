using EcsR3.Components;
using EcsR3.Components.Database;

namespace EcsR3.Extensions
{
    public static class IComponentDatabaseExtensions
    {
        public static IComponent Get(this IComponentDatabase componentDatabase, int allocationIndex, int componentTypeId)
        { return componentDatabase.Get<IComponent>(componentTypeId, allocationIndex); }
    }
}