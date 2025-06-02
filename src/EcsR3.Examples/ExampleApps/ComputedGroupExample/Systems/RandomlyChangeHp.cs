using System;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems.Reactive;
using R3;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample.Systems
{
    public class RandomlyChangeHpGroupSystem : IReactToGroupSystem
    {
        private const int HealthChange = 20;
        
        public IGroup Group { get; } = new Group(typeof(HasHealthComponent));
        private Random _random = new Random();
        
        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(x => observableGroup); }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var healthComponent = entityComponentAccessor.GetComponent<HasHealthComponent>(entity);

            var healthChange = CreateRandomHealthChange();
            healthComponent.CurrentHealth += healthChange;

            if (healthComponent.CurrentHealth <= 0 || healthComponent.CurrentHealth > healthComponent.MaxHealth)
            { healthComponent.CurrentHealth = healthComponent.MaxHealth; }            
        }

        public int CreateRandomHealthChange()
        { return _random.Next(-HealthChange, 0); }
    }
}