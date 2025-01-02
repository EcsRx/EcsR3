using System;
using EcsR3.Examples.Application;
using EcsR3.Examples.ExampleApps.HelloWorldExample.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.ExampleApps.HelloWorldExample
{
    public class HelloWorldExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarted()
        {
            var defaultPool = EntityDatabase.GetCollection();
            var entity = defaultPool.CreateEntity();

            var canTalkComponent = new CanTalkComponent {Message = "Hello world"};
            entity.AddComponents(canTalkComponent);

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