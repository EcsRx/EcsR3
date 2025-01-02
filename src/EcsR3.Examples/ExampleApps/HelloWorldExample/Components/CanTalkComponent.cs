using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.HelloWorldExample.Components
{
    public class CanTalkComponent : IComponent
    {
        public string Message { get; set; }
    }
}