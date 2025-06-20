﻿using System;
using System.Text;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Examples.ExampleApps.HealthExample.Components;
using EcsR3.Examples.Extensions;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems;
using EcsR3.Systems.Reactive;
using R3;

namespace EcsR3.Examples.ExampleApps.HealthExample.Systems
{
    public class DisplayHealthChangesSystem : IReactToDataSystem<float>
    {
        public IGroup Group => new Group(typeof(HealthComponent));
        private const int HealthSegments = 10;

        public Observable<float> ReactToData(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var healthComponent = entityComponentAccessor.GetComponent<HealthComponent>(entity);
            return healthComponent.Health.WithValueChange().Select(CalculateDamageTaken);
        }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity, float damageDone)
        {
            var healthComponent = entityComponentAccessor.GetComponent<HealthComponent>(entity);

            Console.Clear();
            DisplayHealth(healthComponent, damageDone);

            if (healthComponent.Health.Value > 0)
            { Console.WriteLine("Press Space To Attack"); }
            else
            {
                Console.WriteLine("Enemy Is Dead! Hooray etc");
                Console.WriteLine(" - Press Escape To Quit -");
                entityComponentAccessor.RemoveComponent(entity, healthComponent);
            }
        }

        private static float CalculateDamageTaken(ValueChanges<float> values)
        {
            if (values.PreviousValue == 0) { return 0; }
            return values.PreviousValue - values.CurrentValue;
        }

        private static void DisplayHealth(HealthComponent healthComponent, float damageDone)
        {
            var healthPercentage = (healthComponent.Health.Value / healthComponent.MaxHealth) * 100;
            var healthSegments = (int)(healthPercentage / HealthSegments);

            if (healthSegments == 0 && healthPercentage > 0)
            { healthSegments = 1; }

            var healthText = new StringBuilder();
            for (var i = 0; i < HealthSegments; i++)
            {
                var hasSegment = i < healthSegments;
                healthText.Append(hasSegment ? "=" : " ");
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Health");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{healthText}]");
            Console.ResetColor();
            Console.WriteLine();

            if (damageDone >= 1)
            {
                Console.WriteLine($"You did {(int)damageDone} damage to the enemy");
                Console.WriteLine();
            }
        }
    }
}