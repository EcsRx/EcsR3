using System.Linq;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using NSubstitute;
using Xunit;

namespace SystemsR3.Tests.SystemsRx
{
    public class ISystemExtensionTests
    {
        public interface HasMultipleImplementations : IReactToEventSystem<int>, IReactToEventSystem<float>{}

        [Fact]
        public void should_get_interface_generic_type_from_reactive_data_system()
        {
            var fakeSystem = Substitute.For<IReactToEventSystem<int>>();
            var genericTypes = fakeSystem.GetGenericDataTypes(typeof(IReactToEventSystem<>)).ToArray();
            
            Assert.Equal(1, genericTypes.Length);
            Assert.Contains(typeof(int), genericTypes);
        }
        
        [Fact]
        public void should_get_multiple_interface_generic_types_from_reactive_data_system()
        {
            var fakeSystem = Substitute.For<HasMultipleImplementations>();
            var genericTypes = fakeSystem.GetGenericDataTypes(typeof(IReactToEventSystem<>)).ToArray();
            
            Assert.Equal(2, genericTypes.Length);
            Assert.Contains(typeof(int), genericTypes);
            Assert.Contains(typeof(float), genericTypes);
        }
    }
}