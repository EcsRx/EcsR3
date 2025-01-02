using System;
using EcsR3.Examples.Custom.Components;
using EcsR3.Groups;

namespace EcsR3.Examples.Custom.Groups
{
    class MessageGroup : IGroup
    {
        public Type[] RequiredComponents { get;  } = {typeof(FirstComponent) };

        public Type[] ExcludedComponents { get; } = Array.Empty<Type>();
    }
}
