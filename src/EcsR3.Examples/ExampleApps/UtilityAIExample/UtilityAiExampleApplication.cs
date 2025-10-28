using System;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Blueprints;
using EcsR3.Examples.ExampleApps.UtilityAIExample.Modules;
using EcsR3.Extensions;
using SystemsR3.Infrastructure.Extensions;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample
{
    public class UtilityAiExampleApplication : EcsR3ConsoleApplication
    {
        private bool _quit;
        private readonly Random _random = new Random();

        protected override void LoadModules()
        {
            base.LoadModules();
            DependencyRegistry.LoadModule<AIExampleModule>();
        }

        protected override void ApplicationStarted()
        {
            EntityCollection.Create(EntityComponentAccessor, new CharacterAgentBlueprint("Player 1", 200, 50, 50));
            EntityCollection.Create(EntityComponentAccessor, new CharacterAgentBlueprint("Player 2", 300, 75, 0));
            EntityCollection.Create(EntityComponentAccessor, new CharacterAgentBlueprint("Player 3", 120, 0, 75f));

            HandleInput();
        }

        private void HandleInput()
        {
            while (!_quit)
            {
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }
        }
    }
}