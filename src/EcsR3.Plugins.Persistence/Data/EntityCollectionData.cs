using System.Collections.Generic;

namespace EcsR3.Plugins.Persistence.Data
{
    public class EntityCollectionData
    {
        public List<EntityData> Entities { get; set; }

        public EntityCollectionData()
        {
            Entities = new List<EntityData>();
        }
    }
}