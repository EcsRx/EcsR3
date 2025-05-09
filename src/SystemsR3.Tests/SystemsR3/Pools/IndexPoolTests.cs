using System.Collections.Generic;
using System.Linq;
using SystemsR3.Pools;
using SystemsR3.Pools.Config;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Pools
{
    public class IndexPoolTests
    {
        [Fact]
        public void should_allocate_up_front_ids()
        {
            var poolConfig = new PoolConfig(3, 3);
            var indexPool = new IndexPool(poolConfig);
            Assert.Equal(3, indexPool.AvailableIndexes.Count);
            
            var expectedIdEntries = Enumerable.Range(0, 3).ToArray();
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_correctly_for_new_indexes()
        {
            var explicitNewIndex = 5;
            var poolConfig = new PoolConfig(3, 3);
            var indexPool = new IndexPool(poolConfig);
            indexPool.Expand(explicitNewIndex);

            Assert.Equal(explicitNewIndex+1, indexPool.AvailableIndexes.Count);
            
            var expectedIdEntries = Enumerable.Range(0, explicitNewIndex+1).ToArray();
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_correctly_with_auto_expansion()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var poolConfig = new PoolConfig(originalSize, defaultExpansionAmount);
            var indexPool = new IndexPool(poolConfig);
            indexPool.Expand();

            Assert.Equal(indexPool.AvailableIndexes.Count, expectedSize);

            var expectedIdEntries = Enumerable.Range(1, expectedSize).ToArray();
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_continually_expand_correctly_with_auto_expansion()
        {
            var expectedExpansions = 5;
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = (defaultExpansionAmount * expectedExpansions) + originalSize;
            var expectedIndexEntries = Enumerable.Range(0, expectedSize).ToArray();
            var poolConfig = new PoolConfig(originalSize, defaultExpansionAmount);
            var indexPool = new IndexPool(poolConfig);

            for (var i = 0; i < expectedExpansions; i++)
            { indexPool.Expand(); }

            Assert.Equal(expectedSize, indexPool.AvailableIndexes.Count);
            Assert.All(indexPool.AvailableIndexes, x => expectedIndexEntries.Contains(x));
        }
        
        [Fact]
        public void should_continually_expand_correctly_when_allocating_over_count()
        {
            var expectedAllocations = 200;
            var defaultExpansionAmount = 5;
            var originalSize = 10;
            var expectedIndexEntries = Enumerable.Range(0, expectedAllocations).ToArray();
            var actualIndexEntries = new List<int>();
            var poolConfig = new PoolConfig(originalSize, defaultExpansionAmount);
            var indexPool = new IndexPool(poolConfig);

            for (var i = 0; i < expectedAllocations; i++)
            {
                var index = indexPool.AllocateInstance();
                actualIndexEntries.Add(index);
            }

            Assert.Equal(0, indexPool.AvailableIndexes.Count);
            Assert.Equal(expectedAllocations, actualIndexEntries.Count);
            Assert.All(indexPool.AvailableIndexes, x => expectedIndexEntries.Contains(x));
        }
        
        [Fact]
        public void should_allocate_and_remove_next_available_index()
        {
            var poolConfig = new PoolConfig(10, 10);
            var indexPool = new IndexPool(poolConfig);
            var index = indexPool.AllocateInstance();
            
            Assert.InRange(index, 0, 10);
            Assert.DoesNotContain(index, indexPool.AvailableIndexes);
        }
        
        [Fact]
        public void should_expand_and_allocate_and_remove_next_available_index_when_empty()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var poolConfig = new PoolConfig(originalSize, defaultExpansionAmount);
            var indexPool = new IndexPool(poolConfig);
            indexPool.Clear();
            var index = indexPool.AllocateInstance();

            var expectedIdEntries = Enumerable.Range(0, expectedSize).ToList();

            Assert.InRange(index, 0, expectedSize);
            Assert.DoesNotContain(index, indexPool.AvailableIndexes);
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_release_index_when_allocated()
        {
            var poolConfig = new PoolConfig(10, 10);
            var indexPool = new IndexPool(poolConfig);
            var index = indexPool.AllocateInstance();
            indexPool.ReleaseInstance(index);
            
            Assert.Contains(index, indexPool.AvailableIndexes);
        }
        
        [Fact]
        public void should_ignore_index_if_not_within_threshold_when_releasing()
        {
            var poolConfig = new PoolConfig(10, 10);
            var indexPool = new IndexPool(poolConfig);
            var index = 100;
            indexPool.ReleaseInstance(index);
            
            Assert.DoesNotContain(index, indexPool.AvailableIndexes);
        }
        
        [Fact]
        public void should_ignore_duplicated_release_calls_for_same_index()
        {
            var poolConfig = new PoolConfig(10, 10);
            var indexPool = new IndexPool(poolConfig);
            var index = indexPool.AllocateInstance();
            indexPool.ReleaseInstance(index);
            indexPool.ReleaseInstance(index);
            indexPool.ReleaseInstance(index);
            
            Assert.Contains(index, indexPool.AvailableIndexes);
        }
    }
}