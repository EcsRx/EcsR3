using System.Collections.Generic;
using EcsR3.Components.Lookups;
using EcsR3.Entities;
using EcsR3.Entities.Routing;
using NSubstitute;
using R3;
using Xunit;

namespace EcsR3.Tests.EcsR3;

public class EntityChangeRouterTests
{
    [Fact]
    public void should_fire_subscription_with_correct_component_and_entity_data()
    {
        var mockComponentTypeLookup = Substitute.For<IComponentTypeLookup>();
        mockComponentTypeLookup.AllComponentTypeIds.Returns(new[] {1, 2, 3, 4});
        var entityChangeRouter = new EntityChangeRouter(mockComponentTypeLookup);

        var addOneTwoObservable = entityChangeRouter.OnEntityAddedComponents(1, 2);
        var addOneTwoInvocations = new List<(Entity Entity, int[] ComponentIds)>();
        addOneTwoObservable.Subscribe(x => addOneTwoInvocations.Add((x.Entity, x.ComponentIds.ToArray())));
        
        var addThreeFourObservable = entityChangeRouter.OnEntityAddedComponents(3, 4);
        var addThreeFourInvocations = new List<(Entity Entity, int[] ComponentIds)>();
        addThreeFourObservable.Subscribe(x => addThreeFourInvocations.Add((x.Entity, x.ComponentIds.ToArray())));

        var entity = new Entity(1, 0);
        entityChangeRouter.PublishEntityAddedComponents(entity, new[] {1, 3, 4});
        entityChangeRouter.PublishEntityAddedComponents(entity, new[] {2});
        
        Assert.Equal(2, addOneTwoInvocations.Count);
        Assert.Equal(1, addOneTwoInvocations[0].Entity.Id);
        Assert.Equal(1, addOneTwoInvocations[0].ComponentIds.Length);
        Assert.Contains(1, addOneTwoInvocations[0].ComponentIds);
        Assert.Equal(1, addOneTwoInvocations[1].Entity.Id);
        Assert.Equal(1, addOneTwoInvocations[1].ComponentIds.Length);
        Assert.Contains(2, addOneTwoInvocations[1].ComponentIds);
        
        Assert.Equal(1, addThreeFourInvocations.Count);
        Assert.Equal(1, addThreeFourInvocations[0].Entity.Id);
        Assert.Equal(2, addThreeFourInvocations[0].ComponentIds.Length);
        Assert.Contains(3, addThreeFourInvocations[0].ComponentIds);
        Assert.Contains(4, addThreeFourInvocations[0].ComponentIds);
        
    }
}