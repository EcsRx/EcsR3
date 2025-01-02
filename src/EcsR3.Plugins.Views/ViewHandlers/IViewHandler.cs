namespace EcsR3.Plugins.Views.ViewHandlers
{
    public interface IViewHandler
    {
        void DestroyView(object view);
        void SetActiveState(object view, bool isActive);
        object CreateView();
    }
}