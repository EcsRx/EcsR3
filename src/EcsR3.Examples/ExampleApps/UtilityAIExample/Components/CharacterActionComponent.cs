using EcsR3.Components;

namespace EcsR3.Examples.ExampleApps.UtilityAIExample.Components;

public class CharacterActionComponent : IComponent
{
    public string CurrentAction { get; set; } = "Thinking...";
}