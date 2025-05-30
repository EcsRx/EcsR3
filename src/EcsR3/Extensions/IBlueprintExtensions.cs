using EcsR3.Blueprints;
using EcsR3.Entities.Accessors;

namespace EcsR3.Extensions
{
    public static class IBlueprintExtensions
    {
        public static void ApplyToAll(this IBlueprint blueprint,  IEntityComponentAccessor accessor, params int[] entityIds)
        {
            for (var i = 0; i < entityIds.Length; i++)
            { blueprint.Apply(accessor, entityIds[i]); }
        }
    }
}