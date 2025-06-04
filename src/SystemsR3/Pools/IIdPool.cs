namespace SystemsR3.Pools
{
    public interface IIdPool : IPool<int>, IBatchPool<int>
    {
        bool IsAvailable(int id);
        void AllocateSpecificId(int id);
    }
}