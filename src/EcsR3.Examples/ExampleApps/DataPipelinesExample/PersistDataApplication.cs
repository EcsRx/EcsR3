using System;
using SystemsR3.Infrastructure.Extensions;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsR3.Examples.ExampleApps.DataPipelinesExample.Modules;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.DataPipelinesExample
{
    public class PersistDataApplication : EcsR3ConsoleApplication
    {
        private bool _quit;

        protected override void LoadModules()
        {
            base.LoadModules();
            DependencyRegistry.LoadModule<PipelineModule>();
        }

        protected override void ApplicationStarted()
        {
            var entityId = EntityCollection.Create();

            var component = new PlayerStateComponent
            {
                Name = "Super Player 1", 
                Level = 10, 
                SomeFieldThatWontBePersisted = "Wont Be Persisted"
            };
            EntityComponentAccessor.AddComponent(entityId, component);

            Console.WriteLine("This app posts your player state over HTTP which gets echoed back to you.");
            Console.WriteLine("This is a very useful thing if you use online apis like playfab etc");
            Console.WriteLine(" - Press Enter To Trigger Pipeline");
            Console.WriteLine(" - Press Escape To Quit");
            
            HandleInput();
        }

        private void HandleInput()
        {
            while (!_quit)
            {
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Enter)
                {
                    var eventArg = new SavePipelineEvent();
                    EventSystem.Publish(eventArg);
                }
                else if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }
        }
    }
}