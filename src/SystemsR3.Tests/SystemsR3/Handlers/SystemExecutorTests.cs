﻿using System.Text;
using SystemsR3.Exceptions;
using SystemsR3.Executor;
using SystemsR3.Executor.Handlers;
using SystemsR3.Systems;
using NSubstitute;
using SystemsR3.Tests.Systems.Handlers;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Handlers
{
    public class SystemExecutorTests
    {
        [Fact]
        public void should_handle_and_expose_system()
        {
            var fakeSetupSystemHandler = Substitute.For<IConventionalSystemHandler>();
            var fakeSystem = Substitute.For<ISystem>();

            fakeSetupSystemHandler.CanHandleSystem(Arg.Is(fakeSystem)).Returns(true);
           
            var systemExecutor = new SystemExecutor(new[] { fakeSetupSystemHandler });
            systemExecutor.AddSystem(fakeSystem);
            
            fakeSetupSystemHandler.Received(1).SetupSystem(fakeSystem);
            Assert.Contains(fakeSystem, systemExecutor.Systems);
        }
        
        [Fact]
        public void should_handle_and_remove_system()
        {
            var fakeSetupSystemHandler = Substitute.For<IConventionalSystemHandler>();
            var fakeSystem = Substitute.For<ISystem>();

            fakeSetupSystemHandler.CanHandleSystem(Arg.Is(fakeSystem)).Returns(true);
           
            var systemExecutor = new SystemExecutor(new[] { fakeSetupSystemHandler });
            systemExecutor._systems.Add(fakeSystem);
            
            systemExecutor.RemoveSystem(fakeSystem);
            
            fakeSetupSystemHandler.Received(1).DestroySystem(fakeSystem);
            Assert.Empty(systemExecutor.Systems);
        }
        
        [Fact]
        public void should_return_true_if_system_already_exists()
        {
            var fakeSystem = Substitute.For<ISystem>();
           
            var systemExecutor = new SystemExecutor(new IConventionalSystemHandler[0]);
            systemExecutor._systems.Add(fakeSystem);
            Assert.True(systemExecutor.HasSystem(fakeSystem));
        }
        
        [Fact]
        public void should_return_false_if_system_doesnt_exist()
        {
            var fakeSystem = Substitute.For<ISystem>();
           
            var systemExecutor = new SystemExecutor(new IConventionalSystemHandler[0]);
            Assert.False(systemExecutor.HasSystem(fakeSystem));
        }
        
        [Fact]
        public void should_throw_exception_if_system_already_exists()
        {
            var fakeSystem = Substitute.For<ISystem>();
           
            var systemExecutor = new SystemExecutor(new IConventionalSystemHandler[0]);
            systemExecutor._systems.Add(fakeSystem);

            var exceptionWasRaised = false;
            try
            {
                systemExecutor.AddSystem(fakeSystem);
            }
            catch (SystemAlreadyRegisteredException e)
            {
                exceptionWasRaised = true;
            }
            
            Assert.Single(systemExecutor.Systems);
            Assert.True(exceptionWasRaised);
        }
        
        [Fact]
        public void should_destroy_all_systems_and_handlers_when_disposed()
        {
            var fakeSetupSystemHandler1 = Substitute.For<IConventionalSystemHandler>();
            var fakeSetupSystemHandler2 = Substitute.For<IConventionalSystemHandler>();
            var fakeSystem1 = Substitute.For<ISystem>();
            var fakeSystem2 = Substitute.For<ISystem>();
            var fakeSystem3 = Substitute.For<ISystem>();

            fakeSetupSystemHandler1.CanHandleSystem(Arg.Any<ISystem>()).Returns(true);
            fakeSetupSystemHandler2.CanHandleSystem(Arg.Any<ISystem>()).Returns(true);
           
            var systemExecutor = new SystemExecutor(new[] { fakeSetupSystemHandler1, fakeSetupSystemHandler2 });
            systemExecutor._systems.Add(fakeSystem1);
            systemExecutor._systems.Add(fakeSystem2);
            systemExecutor._systems.Add(fakeSystem3);
            
            systemExecutor.Dispose();
            
            fakeSetupSystemHandler1.Received(3).DestroySystem(Arg.Any<ISystem>());
            fakeSetupSystemHandler2.Received(3).DestroySystem(Arg.Any<ISystem>());
            fakeSetupSystemHandler1.Received(1).Dispose();
            fakeSetupSystemHandler2.Received(1).Dispose();

            Assert.Empty(systemExecutor._systems);
        }

        [Fact]
        public void should_run_systems_in_correct_priority()
        {
            var expectedOrder = "1234";
            var actualOrder = new StringBuilder();
            var conventionalSystems = new IConventionalSystemHandler[]
            {
                new DefaultPriorityHandler(() => actualOrder.Append(3)), 
                new HigherPriorityHandler(() => actualOrder.Append(1)), 
                new HighPriorityHandler(() => actualOrder.Append(2)), 
                new LowerPriorityHandler(() => actualOrder.Append(4))
            };
            var systemExecutor = new SystemExecutor(conventionalSystems);
            
            var fakeSystem1 = Substitute.For<ISystem>();
            systemExecutor.AddSystem(fakeSystem1);
            
            Assert.Equal(expectedOrder, actualOrder.ToString());
        }
    }
}