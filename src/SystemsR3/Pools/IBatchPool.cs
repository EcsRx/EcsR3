namespace SystemsR3.Pools
{
    public interface IBatchPool<T>
    {
        T[] AllocateMany(int count);
        void ReleaseMany(T[] instances);
    }
}