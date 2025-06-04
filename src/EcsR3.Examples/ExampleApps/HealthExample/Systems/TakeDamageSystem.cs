using SystemsR3.Systems.Conventional;
using EcsR3.Examples.ExampleApps.HealthExample.Events;
using R3;

namespace EcsR3.Examples.ExampleApps.HealthExample.Systems
{
    public class TakeDamageSystem : IReactToEventSystem<EntityDamagedEvent>
    {
        public Observable<EntityDamagedEvent> ObserveOn(Observable<EntityDamagedEvent> observable)
        { return observable; }

        public void Process(EntityDamagedEvent eventData)
        { eventData.HealthComponent.Health.Value -= eventData.DamageApplied; }
    }
}