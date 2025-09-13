using System;
using CommunityToolkit.HighPerformance;
using EcsR3.Collections.Entities.Pools;
using EcsR3.Components.Database;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Helpers;
using R3;

namespace EcsR3.Collections.Entities
{
    public class EntityAllocationDatabase : IEntityAllocationDatabase, IDisposable
    {
        public const int NoAllocation = IEntityAllocationDatabase.NoAllocation;
        
        public IEntityIdPool EntityIdPool { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public ICreationHasher CreationHasher { get; }
        
        public int ComponentLength { get; protected set; }
        public int EntityLength { get; protected set; }

        private CompositeDisposable _subs = new CompositeDisposable();
        private object _lock = new object();

        /// <summary>
        /// Contains the creation hashes for active entities
        /// </summary>
        public int[] EntityCreationHashes;
        
        /// <summary>
        /// Contains all component allocations for all entities
        /// </summary>
        /// <remarks>First dimension is component type id, second dimension is entity id</remarks>
        public int[,] ComponentAllocationData;

        public EntityAllocationDatabase(IEntityIdPool entityIdPool, IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, ICreationHasher creationHasher)
        {
            EntityIdPool = entityIdPool;
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            CreationHasher = creationHasher;
            EntityLength = EntityIdPool.Size+1;

            EntityIdPool.OnSizeChanged
                .Subscribe(ResizeAllEntityAllocations)
                .AddTo(_subs);
            
            ComponentLength = componentTypeLookup.AllComponentTypeIds.Length;
            ComponentAllocationData = new int[ComponentLength, EntityLength];
            EntityCreationHashes = new int[EntityLength];
            new Span2D<int>(ComponentAllocationData).Fill(NoAllocation);
            ResizeAllEntityAllocations(EntityLength);
        }

        public Entity AllocateEntity(int? id = null)
        {
            int entityId;

            if (id.HasValue)
            {
                if (id.Value > EntityIdPool.Size)
                {
                    var newEntityLength = id.Value + EntityIdPool.PoolConfig.ExpansionSize;
                    EntityIdPool.Expand(newEntityLength);
                    entityId = id.Value;
                }
                else
                {
                    if (EntityCreationHashes[id.Value] == 0)
                    { entityId = id.Value; }
                    else
                    { throw new Exception($"Cannot allocate entity, Id [{id.Value}] is already in use"); }
                }
            }
            else
            { entityId = EntityIdPool.Allocate(); }

            EntityIdPool.AllocateSpecificId(entityId);

            var creationHash = CreationHasher.GenerateHash();
            EntityCreationHashes[entityId] = creationHash;
            return new Entity(entityId, creationHash);
        }

        public Entity[] AllocateEntities(int count)
        {
            var ids = EntityIdPool.AllocateMany(count);
            var creationHash = CreationHasher.GenerateHash();
            
            var entities = new Entity[count];
            for (var i = 0; i < ids.Length; i++)
            {
                var entityId = ids[i];
                entities[i] = new Entity(entityId, creationHash); 
                EntityCreationHashes[entityId] = creationHash;
            }
            return entities;
        }

        public void ReleaseEntity(Entity entity)
        {
            if(entity.CreationHash != EntityCreationHashes[entity.Id]) { return; }
            
            for (var componentTypeId = 0; componentTypeId < ComponentLength; componentTypeId++)
            {
                int allocationId;
                lock (_lock)
                { allocationId = ComponentAllocationData[componentTypeId, entity.Id]; }
                
                if(allocationId == NoAllocation) { continue; }
                ReleaseComponent(componentTypeId, entity);
            }
            
            EntityCreationHashes[entity.Id] = 0;
        }

        public Entity? GetEntity(int entityId)
        {
            if (entityId > EntityCreationHashes.Length)
            { return null; }
            
            var creationHash = EntityCreationHashes[entityId];
            if(creationHash == 0) { return null; }
            
            return new Entity(entityId, EntityCreationHashes[entityId]);
        }
        
        public int AllocateComponent(int componentTypeId, Entity entity)
        {
            int allocationId;
            lock (_lock)
            {
                allocationId = ComponentAllocationData[componentTypeId, entity.Id];
                if(allocationId != NoAllocation) { return allocationId;}

                allocationId = ComponentDatabase.Allocate(componentTypeId);
                ComponentAllocationData[componentTypeId, entity.Id] = allocationId;
            }

            return allocationId;
        }
        
        public int[] AllocateComponents(int[] componentTypeIds, Entity entity)
        {
            var allocationIds = new int[componentTypeIds.Length];
            lock (_lock)
            {
                for (var i = 0; i < componentTypeIds.Length; i++)
                {
                    var componentTypeId = componentTypeIds[i];
                    var allocationId = ComponentAllocationData[componentTypeId, entity.Id];
                    if(allocationId != NoAllocation)
                    { 
                        allocationIds[i] = allocationId; 
                        continue;
                    }
                    
                    allocationId = ComponentDatabase.Allocate(componentTypeId);
                    ComponentAllocationData[componentTypeId, entity.Id] = allocationId;
                    allocationIds[i] = allocationId;
                }
            }
            return allocationIds;
        }
        
        public int[] AllocateComponent(int componentTypeId, Entity[] entities)
        {
            if(entities.Length > EntityLength) 
            { ResizeAllEntityAllocations(entities.Length); }
            
            lock (_lock)
            {
                var newAllocations = ComponentDatabase.Allocate(componentTypeId, entities.Length);
                
                for (var i = 0; i < entities.Length; i++)
                {
                    var entityId = entities[i];
                    var existingAllocation = ComponentAllocationData[componentTypeId, entityId.Id];

                    if (existingAllocation != NoAllocation)
                    { ComponentDatabase.Remove(componentTypeId, existingAllocation); }


                    var newAllocation = newAllocations[i];
                    ComponentAllocationData[componentTypeId, entityId.Id] = newAllocation;
                }

                return newAllocations;
            }
        }

        public bool HasComponent(int componentTypeId, Entity entity)
        {
            lock (_lock)
            { return ComponentAllocationData[componentTypeId, entity.Id] != NoAllocation; }
        }

        public int ReleaseComponent(int componentTypeId, Entity entity)
        {
            int allocationId;
            lock (_lock)
            { 
                allocationId = ComponentAllocationData[componentTypeId, entity.Id];
                if(allocationId == NoAllocation) { return NoAllocation; }

                ComponentAllocationData[componentTypeId, entity.Id] = NoAllocation;
                ComponentDatabase.Remove(componentTypeId, allocationId);
            }
            return allocationId;
        }
        
        public int[] ReleaseComponent(int componentTypeId, Entity[] entities)
        {
            var allocationIds = new int[entities.Length];
            lock (_lock)
            {
                for (var i = 0; i < entities.Length; i++)
                {
                    var entityId = entities[i];
                    allocationIds[i] = ComponentAllocationData[componentTypeId, entityId.Id];
                    if(allocationIds[i] == NoAllocation) { continue; }

                    ComponentAllocationData[componentTypeId, entityId.Id] = NoAllocation;
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
                Array.Resize(ref EntityCreationHashes, newEntityLength);
                EntityLength = newEntityLength + 1;
            }
        }
        
        public int[] GetEntityAllocations(Entity entity)
        {
            var spanData = new Span2D<int>(ComponentAllocationData);
            return spanData.GetColumn(entity.Id).ToArray();
        }
        
        public int[] GetAllocatedComponentTypes(Entity entity)
        {
            Span<int> possibleComponents = stackalloc int[ComponentLength];
            var usedIndexes = 0;
            
            var spanData = new ReadOnlySpan2D<int>(ComponentAllocationData);
            var allComponentAllocations = spanData.GetColumn(entity.Id);
            for(var i=0; i<allComponentAllocations.Length; i++)
            {
                if(allComponentAllocations[i] == NoAllocation) { continue; }
                possibleComponents[usedIndexes++] = i;
            }
            
            return possibleComponents[..usedIndexes].ToArray();
        }

        public Entity[] GetEntitiesWithComponent(int componentTypeId)
        {
            ReadOnlySpan2D<int> spanData;
            lock (_lock) { spanData = new ReadOnlySpan2D<int>(ComponentAllocationData); }
            var componentEntities = spanData.GetRow(componentTypeId);
            Span<int> potentialEntities = stackalloc int[EntityLength];
            
            var usedIndexes = 0;
            for (var i = 0; i < componentEntities.Length; i++)
            {
                if(componentEntities[i] == NoAllocation) { continue; }
                potentialEntities[usedIndexes++] = i;
            }

            var actualEntityIds = potentialEntities[..usedIndexes];
            var entities = new Entity[actualEntityIds.Length];
            for (var i = 0; i < actualEntityIds.Length; i++)
            {
                var entityId = actualEntityIds[i];
                entities[i] = new Entity(entityId, EntityCreationHashes[entityId]);
            }

            return entities;
        }
        
        public int[] GetEntitiesWithComponents(int[] componentTypeIds)
        {
            if(componentTypeIds.Length == 0) { return Array.Empty<int>(); }
            var seedingEntities = GetEntitiesWithComponent(componentTypeIds[0]);

            Span<int> potentialEntities = stackalloc int[seedingEntities.Length];
            var usedIndexes = 0;
            lock (_lock)
            {
                for (var i = 0; i < seedingEntities.Length; i++)
                {
                    var match = false;
                    var entityId = seedingEntities[i].Id;
                    for (var j = 1; j < componentTypeIds.Length; j++)
                    {
                        var componentTypeId = componentTypeIds[j]; 
                        if(ComponentAllocationData[componentTypeId, entityId] == NoAllocation) { break; }
                        match = true;
                    }
                    if(!match) { continue; }
                    potentialEntities[usedIndexes++] = entityId;
                }
            }
            return potentialEntities[..usedIndexes].ToArray();
        }

        public int GetEntityComponentAllocation(int componentTypeId, Entity entity)
        {
            lock (_lock)
            { return ComponentAllocationData[componentTypeId, entity.Id]; }
        }

        public void PreAllocate(int count)
        { ResizeAllEntityAllocations(count); }

        public int[] GetEntityComponentAllocation(int componentTypeId, Entity[] entities)
        {
            ReadOnlySpan2D<int> spanData;
            lock (_lock) { spanData = new ReadOnlySpan2D<int>(ComponentAllocationData); }
            var componentColumn = spanData.GetRow(componentTypeId);
            var allocationIds = new int[entities.Length];
            for (var i = 0; i < entities.Length; i++)
            {
                var entityId = entities[i];
                allocationIds[i] = componentColumn[entityId.Id];
            }
            return allocationIds;
        }

        public void Dispose()
        { _subs?.Dispose(); }
    }
}