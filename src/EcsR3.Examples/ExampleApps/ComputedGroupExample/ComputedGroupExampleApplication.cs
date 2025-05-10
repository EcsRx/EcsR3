using System;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Blueprints;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Modules;

namespace EcsR3.Examples.ExampleApps.ComputedGroupExample
{
    public class ComputedGroupExampleApplication : EcsR3ConsoleApplication
    {
        private bool _quit;

        protected override void LoadModules()
        {
            base.LoadModules();
            DependencyRegistry.LoadModule<ComputedModule>();
        }

        protected override void ApplicationStarted()
        {
            EntityCollection.CreateEntity(new CharacterBlueprint("Bob", 200));
            EntityCollection.CreateEntity(new CharacterBlueprint("Tom", 150));
            EntityCollection.CreateEntity(new CharacterBlueprint("Rolf", 150));
            EntityCollection.CreateEntity(new CharacterBlueprint("Mez", 100));
            EntityCollection.CreateEntity(new CharacterBlueprint("TP", 1000));
            EntityCollection.CreateEntity(new CharacterBlueprint("MasterChief", 100));
            EntityCollection.CreateEntity(new CharacterBlueprint("Weakling", 20));

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