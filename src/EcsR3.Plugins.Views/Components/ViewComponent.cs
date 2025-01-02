using EcsR3.Components;

namespace EcsR3.Plugins.Views.Components
{
    public class ViewComponent : IComponent
    {
        public bool DestroyWithView { get; set; }
        public object View { get; set; }
    }
}