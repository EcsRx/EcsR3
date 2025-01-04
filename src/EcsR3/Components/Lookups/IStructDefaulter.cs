using System;

namespace EcsR3.Components.Lookups
{
    public interface IStructDefaulter
    {
        ValueType GetDefault(int index);
        bool IsDefault<T>(T value, int index);
    }
}