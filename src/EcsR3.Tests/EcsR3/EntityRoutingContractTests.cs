using EcsR3.Entities.Routing;
using Xunit;

namespace EcsR3.Tests.EcsR3;

public class EntityRoutingContractTests
{
    [Theory]
    [InlineData(new[] {1, 2}, new[] {1, 2, 3}, new[] {1, 2})]
    [InlineData(new[] {1, 2, 3}, new[] {1, 2}, new[] {1, 2})]
    [InlineData(new[] {1}, new[] {1, 2}, new[] {1})]
    [InlineData(new[] {1,2,3}, new[] {1, 4}, new[] {1})]
    [InlineData(new[] {2, 1}, new[] {1, 2}, new[] {2,1})]
    [InlineData(new[] {2, 1}, new int[0], new int[0])]
    public void should_correctly_return_matching_components_for_contract(int[] componentsRequired, int[] componentsProvided, int[] expectedOverlap)
    {
        var componentContract = new ComponentContract(componentsRequired);
        var overlap = componentContract.GetMatchingComponentIds(componentsProvided);
        Assert.Equal(expectedOverlap, overlap);
    }
    
    [Theory]
    [InlineData(new[] {1, 2}, new[] {1, 2, 3}, new[] {1, 2})]
    [InlineData(new[] {1, 2, 3}, new[] {1, 2}, new[] {1, 2})]
    [InlineData(new[] {1}, new[] {1, 2}, new[] {1})]
    [InlineData(new[] {1,2,3}, new[] {1, 4}, new[] {1})]
    [InlineData(new[] {2, 1}, new[] {1, 2}, new[] {2,1})]
    [InlineData(new[] {2, 1}, new int[0], new int[0])]
    public void should_correctly_return_matching_components_for_contract_with_no_alloc_version(int[] componentsRequired, int[] componentsProvided, int[] expectedOverlapContains)
    {
        var componentContract = new ComponentContract(componentsRequired);
        var buffer = new int[componentsProvided.Length];
        var lastIndexUsed = componentContract.GetMatchingComponentIdsNoAlloc(componentsProvided, buffer);
        Assert.Equal(lastIndexUsed, expectedOverlapContains.Length-1);
        foreach (var expectedValue in expectedOverlapContains)
        { Assert.Contains(expectedValue, buffer); }
    }
    
    [Fact]
    public void should_correctly_update_buffer_between_calls_to_non_alloc_version()
    {
        var componentContract1 = new ComponentContract([1,2]);
        var componentContract2 = new ComponentContract([22]);
        var componentContract3 = new ComponentContract([4]);
        var providedComponents = new[] {1, 2, 3, 4};
        
        var buffer = new int[providedComponents.Length];
        var contract1LastIndex = componentContract1.GetMatchingComponentIdsNoAlloc(providedComponents, buffer);
        Assert.Equal(1, buffer[0]);
        Assert.Equal(2, buffer[1]);
        Assert.Equal(1, contract1LastIndex);
        
        var contract2LastIndex = componentContract2.GetMatchingComponentIdsNoAlloc(providedComponents, buffer);
        Assert.Equal(1, buffer[0]);
        Assert.Equal(2, buffer[1]);
        Assert.Equal(-1, contract2LastIndex);
        
        var contract3LastIndex = componentContract3.GetMatchingComponentIdsNoAlloc(providedComponents, buffer);
        Assert.Equal(4, buffer[0]);
        Assert.Equal(2, buffer[1]);
        Assert.Equal(0, contract3LastIndex);
    }
}