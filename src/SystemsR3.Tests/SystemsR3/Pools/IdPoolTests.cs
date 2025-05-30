using System.Collections.Generic;
using System.Linq;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Pools
{
    public class IdPoolTests
    {
        [Fact]
        public void should_allocate_up_front_ids()
        {
            var poolConfig = new PoolConfig(10, 10);
            var idPool = new IdPool(poolConfig);
            Assert.Equal(10, idPool.AvailableIds.Count);
        }
        
        [Fact]
        public void should_expand_correctly_for_explicit_id()
        {
            var explicitNewId = 30;
            var poolConfig = new PoolConfig(10, 10);
            var idPool = new IdPool(poolConfig);
            idPool.Expand(explicitNewId);

            Assert.Equal(idPool.AvailableIds.Count, explicitNewId);

            var expectedIdEntries = Enumerable.Range(1, explicitNewId - 1).ToArray();
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_correctly_with_auto_expansion()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var poolConfig = new PoolConfig(originalSize, defaultExpansionAmount);
            var idPool = new IdPool(poolConfig);
            idPool.Expand();

            Assert.Equal(idPool.AvailableIds.Count, expectedSize);

            var expectedIdEntries = Enumerable.Range(1, expectedSize-1).ToArray();
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_allocate_and_remove_next_available_id()
        {
            var poolConfig = new PoolConfig(10, 10);
            var idPool = new IdPool(poolConfig);
            var id = idPool.Allocate();
            
            Assert.InRange(id, 1, 10);
            Assert.DoesNotContain(id, idPool.AvailableIds);
        }
        
        [Fact]
        public void should_batch_allocate_and_remove_next_available_id()
        {
            var allocationSize = 10;
            var poolConfig = new PoolConfig(10, 10);
            var idPool = new IdPool(poolConfig);
            
            var expectedIds = Enumerable.Range(1, allocationSize).ToArray();
            var ids = idPool.AllocateMany(allocationSize);
            
            Assert.Equal(expectedIds, ids);
            for (var i = 0; i < ids.Length; i++)
            { Assert.DoesNotContain(ids[i], idPool.AvailableIds); }
        }
        
        [Fact]
        public void should_expand_and_allocate_and_remove_next_available_id_when_empty()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var poolConfig = new PoolConfig(originalSize, defaultExpansionAmount);
            var idPool = new IdPool(poolConfig);
            idPool.AvailableIds.Clear();
            var id = idPool.Allocate();

            var expectedIdEntries = Enumerable.Range(1, expectedSize-1).ToList();

            Assert.InRange(id, 1, expectedSize);
            Assert.DoesNotContain(id, idPool.AvailableIds);
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_and_allocate_and_remove_specific_id_when_claiming_bigger_than_available()
        {
            var explicitNewId = 30;
            var poolConfig = new PoolConfig(10, 10);
            var idPool = new IdPool(poolConfig);
            idPool.AllocateSpecificId(explicitNewId);

            Assert.Equal(idPool.AvailableIds.Count, explicitNewId-1);

            var expectedIdEntries = Enumerable.Range(1, explicitNewId - 2).ToArray();
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }

        [Fact]
        public void should_correctly_keep_expanding_when_continually_allocating()
        {
            var expectedSize = 5000;
            var idPool = new IdPool();
            var expectedAllocations = Enumerable.Range(1, expectedSize).ToList();
            var actualAllocations = new List<int>();
            
            for (var i = 0; i < expectedSize; i++)
            {
                var allocation = idPool.Allocate(); 
                actualAllocations.Add(allocation);
            }

            Assert.Equal(expectedSize, actualAllocations.Count);
            Assert.All(actualAllocations, x => expectedAllocations.Contains(x));
        }
    }
}