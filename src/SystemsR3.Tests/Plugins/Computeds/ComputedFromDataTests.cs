using R3;
using SystemsR3.Extensions;
using SystemsR3.Tests.Plugins.Computeds.Models;
using Xunit;

namespace SystemsR3.Tests.Plugins.Computeds
{
    public class ComputedFromDataTests
    {
        [Fact]
        public void should_populate_on_creation()
        {
            var expectedData = 10;
            var data = new DummyData{Data = expectedData};            
            
            var computedData = new TestComputedFromData(data);
            Assert.Equal(expectedData, computedData.ComputedData);
        }
        
        [Fact]
        public void should_refresh_value_and_raise_event_when_data_changed_and_refreshed_implicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = 10};
            
            var computedData = new TestComputedFromData(data);
            computedData.OnChanged.Subscribe(x => hasNotified = true);

            data.Data = expectedData;
            computedData.ManuallyRefresh.OnNext(Unit.Default);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);      
            Assert.True(hasNotified);      
        }
        
        [Fact]
        public void should_refresh_value_and_raise_event_when_data_changed_and_refreshed_explicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = 10};
            
            var computedData = new TestComputedFromData(data);
            computedData.OnChanged.Subscribe(x => hasNotified = true);

            data.Data = expectedData;
            computedData.RefreshData();

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);      
            Assert.True(hasNotified);      
        }
        
        [Fact]
        public void should_not_refresh_value_when_datasource_changed_but_not_refreshed()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = expectedData};
            
            var computedData = new TestComputedFromData(data);
            computedData.OnChanged.Subscribe(x => hasNotified = true);
            data.Data = 10;

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.False(hasNotified);
        }
        
        [Fact]
        public void should_not_refresh_value_or_notify_when_datasource_not_changed_even_when_refreshed_implicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = expectedData};
            
            var computedData = new TestComputedFromData(data);
            computedData.OnChanged.Subscribe(x => hasNotified = true);
            computedData.ManuallyRefresh.OnNext(Unit.Default);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.False(hasNotified);
        }
                
        [Fact]
        public void should_not_refresh_value_or_notify_when_datasource_not_changed_even_when_refreshed_explicitly()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = expectedData};
            
            var computedData = new TestComputedFromData(data);
            computedData.OnChanged.Subscribe(x => hasNotified = true);
            computedData.RefreshData();

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.False(hasNotified);
        }
    }
}