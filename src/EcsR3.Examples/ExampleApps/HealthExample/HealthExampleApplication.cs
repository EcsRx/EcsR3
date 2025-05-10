using System;
using EcsR3.Entities;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.HealthExample.Blueprints;
using EcsR3.Examples.ExampleApps.HealthExample.Components;
using EcsR3.Examples.ExampleApps.HealthExample.Events;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.HealthExample
{
    public class HealthExampleApplication : EcsR3ConsoleApplication
    {
        private bool _quit;
        private IEntity _enemy;
        private readonly Random _random = new Random();

        protected override void ApplicationStarted()
        {
            _enemy = EntityCollection.CreateEntity(new EnemyBlueprint(100));

            HandleInput();
        }

        private void HandleInput()
        {
            var healthComponent = _enemy.GetComponent<HealthComponent>();

            while (!_quit)
            {
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Spacebar)
                {
                    var eventArg = new EntityDamagedEvent(healthComponent, _random.Next(5, 25));
                    EventSystem.Publish(eventArg);
                }
                else if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }
        }
    }
}