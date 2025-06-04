using System;
using EcsR3.Examples.Application;
using EcsR3.Examples.Custom.SystemPriority.Components;
using EcsR3.Extensions;

namespace EcsR3.Examples.Custom.SystemPriority
{
    public class SetupSystemPriorityApplication : EcsR3ConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarted()
        {
            var entityId = EntityCollection.Create();
            
            EntityComponentAccessor.AddComponents(entityId, new FirstComponent());

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