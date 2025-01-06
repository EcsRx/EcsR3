using R3;
using SystemsR3.Extensions;
using SystemsR3.Tests.Plugins.Computeds.Models;
using Xunit;

namespace SystemsR3.Tests.Plugins.Computeds
{
    public class ComputedFromObservableTests
    {
        [Fact]
        public void should_populate_on_creation()
        {
            var expectedData = 10;
            var data = new ReactiveProperty<int>(expectedData);          
            
            var computedData = new TestComputedFromObservable(data);
            Assert.Equal(expectedData, computedData.CachedData);
        }
        
        [Fact]
        public void should_refresh_value_and_raise_event_when_data_changed_and_refreshed_implicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new ReactiveProperty<int>(10);
            
            var computedData = new TestComputedFromObservable(data);
            computedData.Subscribe(x => hasNotified = true);
            data.Value = expectedData;

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);      
            Assert.True(hasNotified);      
        }
        
        [Fact]
        public void should_refresh_value_and_raise_event_when_data_changed_and_refreshed_explicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new ReactiveProperty<int>(10);
            
            var computedData = new TestComputedFromObservable(data);
            computedData.Subscribe(x => hasNotified = true);
            computedData.RefreshData(expectedData);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);      
            Assert.True(hasNotified);      
        }
        
        [Fact]
        public void should_not_refresh_value_or_notify_when_datasource_not_changed_even_when_refreshed_implicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new ReactiveProperty<int>(expectedData);
            
            var computedData = new TestComputedFromObservable(data);
            computedData.Subscribe(x => hasNotified = true);
            data.OnNext(expectedData);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.False(hasNotified);
        }
                
        [Fact]
        public void should_not_refresh_value_or_notify_when_datasource_not_changed_even_when_refreshed_explicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new ReactiveProperty<int>(expectedData);
            
            var computedData = new TestComputedFromObservable(data);
            computedData.Subscribe(x => hasNotified = true);
            computedData.RefreshData(expectedData);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.False(hasNotified);
        }
    }
}