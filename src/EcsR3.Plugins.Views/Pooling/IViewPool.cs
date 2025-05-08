using SystemsR3.Pools;

namespace EcsR3.Plugins.Views.Pooling
{
    public interface IViewPool : IPool<object>
    {
        void PreAllocate(int? allocationCount = null);
        void DeAllocate(int dellocationCount);
        void EmptyPool();
    }
}