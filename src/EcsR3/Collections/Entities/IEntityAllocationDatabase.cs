namespace EcsR3.Collections.Entities
{
    public interface IEntityAllocationDatabase : IBatchEntityAllocationDatabase
    {
        public const int NoAllocation = -1;
        
        int AllocateEntity(int? id = null);
        int[] AllocateEntities(int count);
        void ReleaseEntity(int entityId);
        int AllocateComponent(int componentTypeId, int entityId);
        int[] AllocateComponents(int[] componentTypeId, int entityId);
        bool HasComponent(int componentTypeId, int entityId);
        int ReleaseComponent(int componentTypeId, int entityId);
        int[] GetEntityAllocations(int entityId);
        int[] GetAllocatedComponentTypes(int entityId);
        int[] GetEntitiesWithComponent(int componentTypeId);
        int GetEntityComponentAllocation(int componentTypeId, int entityId);
        void PreAllocate(int count);
    }
    
    public interface IBatchEntityAllocationDatabase
    {
       int[] AllocateComponent(int componentTypeId, int[] entityIds);
       int[] ReleaseComponent(int componentTypeId, int[] entityIds);
       int[] GetEntityComponentAllocation(int componentTypeId, int[] entityIds);
    }
}