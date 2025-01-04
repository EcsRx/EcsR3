using EcsR3.Examples.ExampleApps.HealthExample.Components;

namespace EcsR3.Examples.ExampleApps.HealthExample.Events
{
    public class EntityDamagedEvent
    {
        public HealthComponent HealthComponent { get; }
        public float DamageApplied { get; }

        public EntityDamagedEvent(HealthComponent healthComponent, float damageApplied)
        {
            HealthComponent = healthComponent;
            DamageApplied = damageApplied;
        }
    }
}