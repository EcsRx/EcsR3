﻿using System;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Blueprints;
using EcsR3.Examples.ExampleApps.BatchedGroupExample.Modules;

namespace EcsR3.Examples.ExampleApps.BatchedGroupExample
{
    public class BatchedGroupExampleApplication : EcsR3ConsoleApplication
    {
        private bool _quit;
        private int _entityCount = 2;

        protected override void LoadModules()
        {
            base.LoadModules();
            DependencyRegistry.LoadModule<CustomComponentLookupsModule>();
        }

        protected override void ApplicationStarted()
        {
            var blueprint = new MoveableBlueprint();
            
            for (var i = 0; i < _entityCount; i++)
            { EntityCollection.CreateEntity(blueprint); }

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