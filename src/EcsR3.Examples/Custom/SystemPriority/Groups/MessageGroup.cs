using System;
using EcsR3.Examples.Custom.SystemPriority.Components;
using EcsR3.Groups;

namespace EcsR3.Examples.Custom.SystemPriority.Groups
{
    class MessageGroup : IGroup
    {
        public Type[] RequiredComponents { get;  } = {typeof(FirstComponent) };

        public Type[] ExcludedComponents { get; } = Array.Empty<Type>();
    }
}
