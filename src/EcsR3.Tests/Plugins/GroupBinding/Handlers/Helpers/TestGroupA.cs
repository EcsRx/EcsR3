using System;
using EcsR3.Groups;
using EcsR3.Tests.Models;

namespace EcsR3.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class TestGroupA : IGroup
    {
        public Type[] RequiredComponents { get; set; } = {typeof(TestComponentOne)};
        public Type[] ExcludedComponents { get; set; } = Array.Empty<Type>();
    }
}