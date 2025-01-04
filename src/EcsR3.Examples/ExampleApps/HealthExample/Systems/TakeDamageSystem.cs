using SystemsR3.Systems.Conventional;
using EcsR3.Examples.ExampleApps.HealthExample.Events;

namespace EcsR3.Examples.ExampleApps.HealthExample.Systems
{
    public class TakeDamageSystem : IReactToEventSystem<EntityDamagedEvent>
    {
        public void Process(EntityDamagedEvent eventData)
        { eventData.HealthComponent.Health.Value -= eventData.DamageApplied; }
    }
}