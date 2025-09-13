using EcsR3.Collections.Entities;
using Xunit;

namespace EcsR3.Tests.EcsR3.Entities;

public class CreationHasherTests
{
    [Fact]
    public void should_give_new_hash_each_time()
    {
        var newHash = new CreationHasher();
        var hash1 = newHash.GenerateHash();
        var hash2 = newHash.GenerateHash();
        
        Assert.NotEqual(hash1, hash2);
    }
    
    [Fact]
    public void should_increment_rolling_value_each_time()
    {
        var newHash = new CreationHasher();
        var currentValue = newHash.RollingHashValue;
        var hash1 = newHash.GenerateHash();
        var hash2 = newHash.GenerateHash();
        var newValue = newHash.RollingHashValue;
        
        Assert.Equal(currentValue+2, newValue);
    }
    
    [Fact]
    public void should_roll_over_if_max_value()
    {
        var newHash = new CreationHasher { RollingHashValue = int.MaxValue };
        var hash1 = newHash.GenerateHash();
        var newValue = newHash.RollingHashValue;
        Assert.Equal(CreationHasher.StartingValue, newValue);
    }
}