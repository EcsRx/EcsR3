using System.Collections.Generic;
using System.Linq;
using EcsR3.Components;
using EcsR3.Systems;

namespace EcsR3.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IGroupSystem> GetApplicableSystems(this IEnumerable<IGroupSystem> systems, IEnumerable<IComponent> components)
        {
            var componentTypes = components.Select(x => x.GetType());
            return systems.Where(x => x.Group.RequiredComponents.All(y => componentTypes.Contains(y)));
        }
    }
}