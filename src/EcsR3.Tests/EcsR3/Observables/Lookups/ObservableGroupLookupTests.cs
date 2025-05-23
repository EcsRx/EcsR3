using System;
using EcsR3.Groups;
using EcsR3.Groups.Observable;
using EcsR3.Lookups;
using NSubstitute;
using Xunit;

namespace EcsR3.Tests.EcsRx.Observables.Lookups
{
    public class ObservableGroupLookupTests
    {
        [Fact]
        public void should_see_lookup_groups_as_equal()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyGroup2 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            Assert.True(dummyGroup1.Equals(dummyGroup2));
        }
        
        [Fact]
        public void should_generate_same_hashcode_for_lookup_groups()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyGroup2 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            Assert.Equal(dummyGroup1.GetHashCode(), dummyGroup2.GetHashCode());
        }
        
        [Fact]
        public void should_see_lookup_groups_as_different()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyGroup2 = new LookupGroup(new[] { 2 }, Array.Empty<int>());
            Assert.False(dummyGroup1.Equals(dummyGroup2));
        }
        
        [Fact]
        public void should_correctly_identify_has_matching_group_already()
        {
            var checkGroup = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyObservableGroup = Substitute.For<IObservableGroup>();
            dummyObservableGroup.Group.Returns(checkGroup);
            
            var lookup = new ObservableGroupLookup { dummyObservableGroup };

            Assert.True(lookup.Contains(checkGroup));
        }
    }
}