using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;

namespace EcsR3.Extensions
{
    public static class IBlueprintExtensions
    {
        public static void ApplyToAll(this IBlueprint blueprint,  IEntityComponentAccessor accessor, params Entity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { blueprint.Apply(accessor, entities[i]); }
        }
    }
}