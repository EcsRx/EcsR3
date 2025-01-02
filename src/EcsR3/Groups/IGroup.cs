using System;

namespace EcsR3.Groups
{
    public interface IGroup
    {
        Type[] RequiredComponents { get; }
        Type[] ExcludedComponents { get; }
    }
}