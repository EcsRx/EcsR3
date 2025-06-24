using System;
using System.Collections.Generic;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsR3.Tests.EcsR3;

public class IEntityComponentAccessorExtensionTests
{
    [Fact]
    public void should_correctly_get_entities_matching_group()
    {
        var dummyGroup = new Group(new[] { typeof(TestComponentOne) }, new[] { typeof(TestComponentTwo) });
        var entityShouldMatch = new Entity(1, 0);
        var entityShouldNotMatchDueToRequired = new Entity(2, 0);
        var entityShouldNotMatchDueToExcluded = new Entity(3, 0);
            
        var mockEntityComponentAccessor = Substitute.For<IEntityComponentAccessor>();
        // Has all required, no excluded ones
        mockEntityComponentAccessor.HasAllComponents(Arg.Is(entityShouldMatch), Arg.Any<IReadOnlyList<Type>>()).Returns(true);
        mockEntityComponentAccessor.HasAnyComponents(Arg.Is(entityShouldMatch), Arg.Any<IReadOnlyList<Type>>()).Returns(false);
        
        // Has all required, has excluded ones
        mockEntityComponentAccessor.HasAllComponents(Arg.Is(entityShouldNotMatchDueToExcluded), Arg.Any<IReadOnlyList<Type>>()).Returns(true);
        mockEntityComponentAccessor.HasAnyComponents(Arg.Is(entityShouldNotMatchDueToExcluded), Arg.Any<IReadOnlyList<Type>>()).Returns(true);
        
        // Missing required, has no excluded ones
        mockEntityComponentAccessor.HasAllComponents(Arg.Is(entityShouldNotMatchDueToRequired), Arg.Any<IReadOnlyList<Type>>()).Returns(false);
        mockEntityComponentAccessor.HasAnyComponents(Arg.Is(entityShouldNotMatchDueToRequired), Arg.Any<IReadOnlyList<Type>>()).Returns(false);
        
        var does1Match = IEntityComponentAccessorExtensions.MatchesGroup(mockEntityComponentAccessor, entityShouldMatch, dummyGroup);
        var does2Match = IEntityComponentAccessorExtensions.MatchesGroup(mockEntityComponentAccessor, entityShouldNotMatchDueToRequired, dummyGroup);
        var does3Match = IEntityComponentAccessorExtensions.MatchesGroup(mockEntityComponentAccessor, entityShouldNotMatchDueToExcluded, dummyGroup);
        
        Assert.True(does1Match);
        Assert.False(does2Match);
        Assert.False(does3Match);
    }
}