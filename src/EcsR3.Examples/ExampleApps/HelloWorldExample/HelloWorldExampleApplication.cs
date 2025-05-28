using System;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.HelloWorldExample.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.HelloWorldExample
{
    public class HelloWorldExampleApplication : EcsR3ConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarted()
        {
            var entityId = EntityCollection.Create();

            var canTalkComponent = new CanTalkComponent {Message = "Hello world"};
            EntityComponentAccessor.AddComponents(entityId, canTalkComponent);

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