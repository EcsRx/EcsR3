using EcsR3.Components;
using SystemsR3.Plugins.Transforms.Models;

namespace EcsR3.Plugins.Transforms.Components
{
    public class Transform2DComponent : IComponent
    {
        /// <summary>
        /// The transform of the component
        /// </summary>
        public Transform2D Transform { get; set; } = new Transform2D();
    }
}