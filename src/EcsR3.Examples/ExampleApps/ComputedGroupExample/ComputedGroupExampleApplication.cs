using System;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Blueprints;
using EcsR3.Examples.ExampleApps.ComputedGroupExample.Modules;
using EcsR3.Extensions;

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
            EntityCollection.Create(new CharacterBlueprint("Bob", 200));
            EntityCollection.Create(new CharacterBlueprint("Tom", 150));
            EntityCollection.Create(new CharacterBlueprint("Rolf", 150));
            EntityCollection.Create(new CharacterBlueprint("Mez", 100));
            EntityCollection.Create(new CharacterBlueprint("TP", 1000));
            EntityCollection.Create(new CharacterBlueprint("MasterChief", 100));
            EntityCollection.Create(new CharacterBlueprint("Weakling", 20));

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