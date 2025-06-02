using EcsR3.Entities;

namespace EcsR3.Collections.Entities
{
    public interface IEntityAllocationDatabase : IBatchEntityAllocationDatabase
    {
        public const int NoAllocation = -1;
        
        Entity? GetEntity(int entityId);
        Entity AllocateEntity(int? id = null);
        Entity[] AllocateEntities(int count);
        void ReleaseEntity(Entity entity);
        int AllocateComponent(int componentTypeId, Entity entity);
        int[] AllocateComponents(int[] componentTypeId, Entity entity);
        bool HasComponent(int componentTypeId, Entity entity);
        int ReleaseComponent(int componentTypeId, Entity entity);
        int[] GetEntityAllocations(Entity entity);
        int[] GetAllocatedComponentTypes(Entity entity);
        Entity[] GetEntitiesWithComponent(int componentTypeId);
        int GetEntityComponentAllocation(int componentTypeId, Entity entity);
        void PreAllocate(int count);
    }
    
    public interface IBatchEntityAllocationDatabase
    {
       int[] AllocateComponent(int componentTypeId, Entity[] entities);
       int[] ReleaseComponent(int componentTypeId, Entity[] entities);
       int[] GetEntityComponentAllocation(int componentTypeId, Entity[] entities);
    }
}