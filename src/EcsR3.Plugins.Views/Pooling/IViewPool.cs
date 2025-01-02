using SystemsR3.Pools;

namespace EcsR3.Plugins.Views.Pooling
{
    public interface IViewPool : IPool<object>
    {
        void PreAllocate(int allocationCount);
        void DeAllocate(int dellocationCount);
        void EmptyPool();
    }
}