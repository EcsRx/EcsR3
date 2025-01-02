using System.Collections.Generic;

namespace EcsR3.Plugins.Persistence.Data
{
    public class EntityDatabaseData
    {
        public List<EntityCollectionData> EntityCollections { get; set; }
        public string Version { get; set; }
        
        public EntityDatabaseData()
        {
            EntityCollections = new List<EntityCollectionData>();
            Version = "1.0.0";
        }
    }
}