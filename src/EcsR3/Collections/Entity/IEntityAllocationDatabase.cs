using System.Collections.Generic;

namespace EcsR3.Collections.Entity
{
    public interface IEntityAllocationDatabase
    {
        public const int NoAllocation = -1;
        
        int AllocateEntity();
        void ReleaseEntity(int entityId);
        int AllocateComponent(int componentTypeId, int entityId);
        int[] AllocateComponents(int[] componentTypeId, int entityId);
        bool HasComponent(int componentTypeId, int entityId);
        int ReleaseComponent(int componentTypeId, int entityId);
        int[] GetEntityAllocations(int entityId);
        int[] GetAllEntityComponents(int entityId);
        int[] GetEntitiesWithComponent(int componentTypeId);
        int GetEntityComponentAllocation(int componentTypeId, int entityId);
    }
}