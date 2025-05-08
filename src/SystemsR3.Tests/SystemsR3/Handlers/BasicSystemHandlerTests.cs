using System;
using NSubstitute;
using SystemsR3.Executor.Handlers.Conventional;
using SystemsR3.Scheduling;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Handlers
{
    public class BasicSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var reactToEntitySystemHandler = new BasicSystemHandler(observableScheduler);
            
            var fakeMatchingSystem = Substitute.For<IBasicSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IManualSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var mockSystem = Substitute.For<IBasicSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new BasicSystemHandler(observableScheduler);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}