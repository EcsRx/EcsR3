using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Extensions
{
    public static class IEntityExtensions
    {
        public static int GetHealthPercentile(this IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var healthComponent = entityComponentAccessor.GetComponent<HasHealthComponent>(entityId);
            var percentile = healthComponent.CurrentHealth / (float) healthComponent.MaxHealth;
            var percentage = percentile * 100;
            return (int) percentage;
        }
        
        public static string GetHealthString(this IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var healthComponent = entityComponentAccessor.GetComponent<HasHealthComponent>(entityId);
            return $"{healthComponent.CurrentHealth}/{healthComponent.MaxHealth}";
        }
        
        public static string GetName(this IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var nameComponent = entityComponentAccessor.GetComponent<HasNameComponent>(entityId);
            return nameComponent.Name;
        }
    }
}