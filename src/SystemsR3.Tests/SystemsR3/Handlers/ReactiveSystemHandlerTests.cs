using System;
using NSubstitute;
using R3;
using SystemsR3.Executor.Handlers.Conventional;
using SystemsR3.Systems;
using SystemsR3.Systems.Conventional;
using Xunit;

namespace SystemsR3.Tests.SystemsRx.Handlers
{
    public class ReactiveSystemHandlerTests
    {
        public interface MultipleOfSameInterface : IReactiveSystem<int>, IReactiveSystem<float>
        {

        }
        
        [Fact]
        public void should_correctly_handle_systems()
        {
            var reactiveSystemHandler = new ReactiveSystemHandler();
            
            var fakeMatchingSystem = Substitute.For<IReactiveSystem<int>>();
            var fakeNonMatchingSystem1 = Substitute.For<ISystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(reactiveSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactiveSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactiveSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var mockSystem = Substitute.For<IReactiveSystem<int>>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactiveSystemHandler();
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }

        [Fact]
        public void should_process_events_with_multiple_interfaces()
        {
            var dummyIntSubject = new Subject<int>();
            var dummyFloatSubject = new Subject<float>();
            
            var mockSystem = Substitute.For<MultipleOfSameInterface>();
            var dummyInt = 100;
            var dummyFloat = 2.5f;

            (mockSystem as IReactiveSystem<int>).ReactTo().Returns(dummyIntSubject);
            (mockSystem as IReactiveSystem<float>).ReactTo().Returns(dummyFloatSubject);
            
            var systemHandler = new ReactiveSystemHandler();
            systemHandler.SetupSystem(mockSystem);
            dummyIntSubject.OnNext(dummyInt);
            dummyFloatSubject.OnNext(dummyFloat);
            
            (mockSystem as IReactiveSystem<int>).Received(1).Execute(Arg.Is(dummyInt));
            (mockSystem as IReactiveSystem<float>).Received(1).Execute(Arg.Is(dummyFloat));
        }
    }
}