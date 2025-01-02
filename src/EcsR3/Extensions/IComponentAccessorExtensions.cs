using EcsR3.Components;
using EcsR3.Components.Accessor;
using EcsR3.Entities;

namespace EcsR3.Extensions
{
    public static class IComponentAccessorExtensions
    {
        public static ref T Add<T>(this IComponentAccessor<T> accessor, IEntity entity)
            where T : IComponent, new() => ref entity.AddComponent<T>(accessor.ComponentTypeId);
    }
}