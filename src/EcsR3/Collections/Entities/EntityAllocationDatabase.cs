using System;
using CommunityToolkit.HighPerformance;
using EcsR3.Collections.Entities.Pools;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities.Routing;
using EcsR3.Helpers;
using R3;

namespace EcsR3.Collections.Entities
{
    public class EntityAllocationDatabase : IEntityAllocationDatabase, IDisposable
    {
        public const int NoAllocation = IEntityAllocationDatabase.NoAllocation;
        
        public IEntityIdPool EntityIdPool { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IEntityChangeRouter EntityChangeRouter { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        public int ComponentLength { get; protected set; }
        public int EntityLength { get; protected set; }

        private CompositeDisposable _subs = new CompositeDisposable();
        private object _lock = new object();

        /// <summary>
        /// Contains all component allocations for all entities
        /// </summary>
        /// <remarks>First dimension is component type id, second dimension is entity id</remarks>
        protected int[,] ComponentAllocationData;

        public EntityAllocationDatabase(IEntityIdPool entityIdPool, IComponentDatabase componentDatabase, IEntityChangeRouter entityChangeRouter, IComponentTypeLookup componentTypeLookup)
        {
            EntityIdPool = entityIdPool;
            ComponentDatabase = componentDatabase;
            EntityChangeRouter = entityChangeRouter;
            ComponentTypeLookup = componentTypeLookup;
            EntityLength = EntityIdPool.Size+1;

            EntityIdPool.OnSizeChanged
                .Subscribe(ResizeAllEntityAllocations)
                .AddTo(_subs);
            
            ComponentLength = componentTypeLookup.AllComponentTypeIds.Length;
            ComponentAllocationData = new int[ComponentLength, EntityLength];
            ResizeAllEntityAllocations(EntityLength);
        }

        public int AllocateEntity(int? id = null)
        {
            if(!id.HasValue)
            { return EntityIdPool.AllocateInstance(); }

            EntityIdPool.AllocateSpecificId(id.Value);
            return id.Value;
        }
        
        public int[] AllocateEntities(int count)
        { return EntityIdPool.Allocate(count); }

        public void ReleaseEntity(int entityId)
        {
            for (var componentTypeId = 0; componentTypeId < ComponentAllocationData.Length; componentTypeId++)
            {
                int allocationId;
                lock (_lock)
                { allocationId = ComponentAllocationData[componentTypeId, entityId]; }
                
                if(allocationId == NoAllocation) { continue; }
                ReleaseComponent(componentTypeId, entityId);
            }
        }

        public int AllocateComponent(int componentTypeId, int entityId)
        {
            int allocationId;
            lock (_lock)
            {
                allocationId = ComponentAllocationData[componentTypeId, entityId];
                if(allocationId != NoAllocation) { return allocationId;}

                allocationId = ComponentDatabase.Allocate(componentTypeId);
                ComponentAllocationData[componentTypeId, entityId] = allocationId;
            }

            return allocationId;
        }
        
        public int[] AllocateComponents(int[] componentTypeIds, int entityId)
        {
            var allocationIds = new int[componentTypeIds.Length];
            lock (_lock)
            {
                for (var i = 0; i < componentTypeIds.Length; i++)
                {
                    var componentTypeId = componentTypeIds[i];
                    var allocationId = ComponentAllocationData[componentTypeId, entityId];
                    if(allocationId != NoAllocation)
                    { 
                        allocationIds[i] = allocationId; 
                        continue;
                    }
                    
                    allocationIds[i] = ComponentDatabase.Allocate(componentTypeId);
                }
            }
            return allocationIds;
        }
        
        public int[] AllocateComponent(int componentTypeId, int[] entityIds)
        {
            lock (_lock)
            {
                var newAllocations = ComponentDatabase.Allocate(componentTypeId, entityIds.Length);
                
                for (var i = 0; i < entityIds.Length; i++)
                {
                    var entityId = entityIds[i];
                    var existingAllocation = ComponentAllocationData[componentTypeId, entityId];

                    if (existingAllocation != NoAllocation)
                    { ComponentDatabase.Remove(componentTypeId, existingAllocation); }


                    var newAllocation = newAllocations[i];
                    ComponentAllocationData[componentTypeId, entityId] = newAllocation;
                }

                return newAllocations;
            }
        }

        public bool HasComponent(int componentTypeId, int entityId)
        {
            lock (_lock)
            { return ComponentAllocationData[componentTypeId, entityId] == NoAllocation; }
        }

        public int ReleaseComponent(int componentTypeId, int entityId)
        {
            int allocationId;
            lock (_lock)
            { 
                allocationId = ComponentAllocationData[componentTypeId, entityId];
                if(allocationId == NoAllocation) { return NoAllocation; }

                ComponentAllocationData[componentTypeId, entityId] = NoAllocation;
                ComponentDatabase.Remove(componentTypeId, allocationId);
            }
            return allocationId;
        }
        
        public int[] ReleaseComponent(int componentTypeId, int[] entityIds)
        {
            var allocationIds = new int[entityIds.Length];
            lock (_lock)
            {
                for (var i = 0; i < entityIds.Length; i++)
                {
                    var entityId = entityIds[i];
                    allocationIds[i] = ComponentAllocationData[componentTypeId, entityId];
                    if(allocationIds[i] == NoAllocation) { continue; }

                    ComponentAllocationData[componentTypeId, entityId] = NoAllocation;
                }
                ComponentDatabase.Remove(componentTypeId, allocationIds);
            }
            return allocationIds;
        }

        public void ResizeAllEntityAllocations(int newEntityLength)
        {
            lock (_lock)
            {
                if(EntityLength >= newEntityLength) { return ; }
                ArrayHelper.Resize2DArray(ref ComponentAllocationData, ComponentLength, newEntityLength, NoAllocation);
                EntityLength = newEntityLength + 1;
            }
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
        {
            lock (_lock)
            { return ComponentAllocationData[componentTypeId, entityId]; }
        }

        public void PreAllocate(int count)
        { ResizeAllEntityAllocations(count); }

        public int[] GetEntityComponentAllocation(int componentTypeId, int[] entityIds)
        {
            var spanData = new Span2D<int>(ComponentAllocationData);
            var componentColumn = spanData.GetColumn(componentTypeId);
            var allocationIds = new int[entityIds.Length];
            for (var i = 0; i < entityIds.Length; i++)
            {
                var entityId = entityIds[i];
                allocationIds[i] = componentColumn[entityId];
            }
            return allocationIds;
        }

        public void Dispose()
        { _subs?.Dispose(); }
    }
}