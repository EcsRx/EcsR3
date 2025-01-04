using EcsR3.Entities;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Extensions
{
    public static class IEntityExtensions
    {
        public static int GetHealthPercentile(this IEntity entity)
        {
            var healthComponent = entity.GetComponent<HasHealthComponent>();
            var percentile = healthComponent.CurrentHealth / (float) healthComponent.MaxHealth;
            var percentage = percentile * 100;
            return (int) percentage;
        }
        
        public static string GetHealthString(this IEntity entity)
        {
            var healthComponent = entity.GetComponent<HasHealthComponent>();
            return $"{healthComponent.CurrentHealth}/{healthComponent.MaxHealth}";
        }
        
        public static string GetName(this IEntity entity)
        {
            var nameComponent = entity.GetComponent<HasNameComponent>();
            return nameComponent.Name;
        }
    }
}