using System;
using EcsR3.Groups;

namespace EcsR3.Plugins.GroupBinding.Attributes
{
    /// <summary>
    /// Will attempt to auto populate an ComputedEntityGroup with a provided required components
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FromComponentsAttribute : Attribute
    {
        public IGroup Group { get; }

        public FromComponentsAttribute(params Type[] requiredComponents)
        { Group = new Group(requiredComponents); }
    }
}