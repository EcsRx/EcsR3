using EcsR3.Components;
using SystemsR3.Plugins.Transforms.Models;

namespace EcsR3.Plugins.Transforms.Components
{
    public class TransformComponent : IComponent
    {
        /// <summary>
        /// The transform of the component
        /// </summary>
        public Transform Transform { get; set; } = new Transform();
    }
}