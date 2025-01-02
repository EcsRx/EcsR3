namespace SystemsR3.Pools
{
    public interface IIdPool : IPool<int>
    {
        bool IsAvailable(int id);
        void AllocateSpecificId(int id);
    }
}