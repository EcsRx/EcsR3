using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.HealthExample.Components;
using EcsR3.Extensions;
using R3;

namespace EcsR3.Examples.ExampleApps.HealthExample.Blueprints
{
    public class EnemyBlueprint : IBlueprint
    {
        public float Health { get; }

        public EnemyBlueprint(float health)
        {
            Health = health;
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, int entityId)
        {
            var healthComponent = new HealthComponent
            {
                Health = new ReactiveProperty<float>(Health),
                MaxHealth = Health
            };
            entityComponentAccessor.AddComponents(entityId, healthComponent);
        }
    }
}