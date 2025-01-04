using System.Collections.Generic;
using EcsR3.Components;

namespace EcsR3.Plugins.Persistence.Data
{
    public class EntityData
    {
        public int EntityId { get; set; }
        public List<IComponent> Components { get; set; }

        public EntityData()
        {
            Components = new List<IComponent>();
        }
    }
}