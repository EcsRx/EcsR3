using System;
using System.Collections.Generic;
using NSubstitute;
using SystemsR3.Extensions;
using Xunit;

namespace SystemsR3.Tests.SystemsR3.Extensions
{
    public class IDictionaryExtensionTests
    {
        [Fact]
        public void should_work_with_nulls_and_disposables_individually()
        {
            var mockDisposable1 = Substitute.For<IDisposable>();
            var mockDisposable2 = Substitute.For<IDisposable>();
            
            var dictionary = new Dictionary<int, IDisposable>
            {
                { 1, null },
                { 2, mockDisposable1 },
                { 3, null },
                { 4, mockDisposable2 }
            };
            
            dictionary.RemoveAndDispose(1);
            dictionary.RemoveAndDispose(2);
            dictionary.RemoveAndDispose(3);
            dictionary.RemoveAndDispose(4);
            
            Assert.Empty(dictionary);
            mockDisposable1.Received(1).Dispose();
            mockDisposable2.Received(1).Dispose();
        }
        
        [Fact]
        public void should_work_with_nulls_and_disposables()
        {
            var mockDisposable1 = Substitute.For<IDisposable>();
            var mockDisposable2 = Substitute.For<IDisposable>();
            
            var dictionary = new Dictionary<int, IDisposable>
            {
                { 1, null },
                { 2, mockDisposable1 },
                { 3, null },
                { 4, mockDisposable2 }
            };
            
            dictionary.RemoveAndDisposeAll();

            Assert.Empty(dictionary);
            mockDisposable1.Received(1).Dispose();
            mockDisposable2.Received(1).Dispose();
        }
        
        [Fact]
        public void should_work_with_nulls_and_disposables_in_enumerable()
        {
            var mockDisposable1 = Substitute.For<IDisposable>();
            var mockDisposable2 = Substitute.For<IDisposable>();

            var collection = new[]
            {
                null,
                mockDisposable1,
                null,
                mockDisposable2
            };
            
            collection.DisposeAll();

            Assert.Equal(4, collection.Length);
            mockDisposable1.Received(1).Dispose();
            mockDisposable2.Received(1).Dispose();
        }
    }
}