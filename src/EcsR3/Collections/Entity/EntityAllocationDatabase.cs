using System;
using CommunityToolkit.HighPerformance;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using EcsR3.Helpers;
using SystemsR3.Pools;

namespace EcsR3.Collections.Entity
{
    public class EntityAllocationDatabase : IEntityAllocationDatabase
    {
        public const int NoAllocation = IEntityAllocationDatabase.NoAllocation;
        
        public IIdPool EntityIdPool { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IEntityChangeRouter EntityChangeRouter { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        public int ComponentLength { get; protected set; }
        public int EntityLength { get; protected set; }

        /// <summary>
        /// Contains all component allocations for all entities
        /// </summary>
        /// <remarks>First dimension is component type id, second dimension is entity id</remarks>
        protected int[,] ComponentAllocationData;

        public EntityAllocationDatabase(IIdPool entityIdPool, IComponentDatabase componentDatabase, IEntityChangeRouter entityChangeRouter, IComponentTypeLookup componentTypeLookup)
        {
            EntityIdPool = entityIdPool;
            ComponentDatabase = componentDatabase;
            EntityChangeRouter = entityChangeRouter;
            ComponentTypeLookup = componentTypeLookup;
            EntityLength = EntityIdPool.Size;
            
            ComponentAllocationData = new int[ComponentLength, EntityIdPool.Size];
            ResizeAllEntityAllocations(EntityIdPool.Size);
        }

        public int AllocateEntity()
        { return EntityIdPool.AllocateInstance(); }

        public void ReleaseEntity(int entityId)
        {
            for (var componentTypeId = 0; componentTypeId < ComponentAllocationData.Length; componentTypeId++)
            {
                var allocationId = ComponentAllocationData[componentTypeId, entityId];
                if(allocationId == NoAllocation) { continue; }
                ReleaseComponent(componentTypeId, entityId);
            }
        }

        public int AllocateComponent(int componentTypeId, int entityId)
        {
            var currentAllocation = ComponentAllocationData[componentTypeId, entityId];
            if(currentAllocation != NoAllocation) { return currentAllocation;}
            
            var allocationId = ComponentDatabase.Allocate(componentTypeId);
            ComponentAllocationData[componentTypeId, entityId] = allocationId;
            return allocationId;
        }

        public bool HasComponent(int componentTypeId, int entityId)
        { return ComponentAllocationData[componentTypeId, entityId] == NoAllocation; }

        public int ReleaseComponent(int componentTypeId, int entityId)
        {
            var allocationId = ComponentAllocationData[componentTypeId, entityId];
            if(allocationId == NoAllocation) { return NoAllocation; }
            
            ComponentAllocationData[componentTypeId, entityId] = NoAllocation;
            ComponentDatabase.Remove(componentTypeId, allocationId);
            return allocationId;
        }

        public void ResizeAllEntityAllocations(int newEntityLength)
        {
            if(EntityLength >= newEntityLength) { return ; }
            ArrayHelper.Resize2DArray(ref ComponentAllocationData, ComponentLength, newEntityLength);
            EntityLength = newEntityLength;
        }
        
        public int[] GetEntityAllocations(int entityId)
        {
            var spanData = new Span2D<int>(ComponentAllocationData);
            return spanData.GetRow(entityId).ToArray();
        }

        public int[] GetAllEntityComponents(int entityId)
        {
            Span<int> possibleComponents = stackalloc int[ComponentLength];
            var usedIndexes = 0;
            
            var spanData = new Span2D<int>(ComponentAllocationData);
            var allComponentAllocations = spanData.GetRow(entityId);
            for(var i=0; i<allComponentAllocations.Length; i++)
            {
                if(allComponentAllocations[i] == NoAllocation) { continue; }
                possibleComponents[usedIndexes++] = i;
            }
            
            return possibleComponents[..usedIndexes].ToArray();
        }

        public int[] GetEntitiesWithComponent(int componentTypeId)
        {
            var spanData = new Span2D<int>(ComponentAllocationData);
            return spanData.GetColumn(componentTypeId).ToArray();
        }
        
        public int GetEntityComponentAllocation(int componentTypeId, int entityId)
        { return ComponentAllocationData[componentTypeId, entityId]; }
        
    }
}