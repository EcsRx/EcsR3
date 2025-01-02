using EcsR3.Blueprints;
using EcsR3.Entities;

namespace EcsR3.Extensions
{
    public static class IBlueprintExtensions
    {
        public static void ApplyToAll(this IBlueprint blueprint, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { blueprint.Apply(entities[i]); }
        }
    }
}