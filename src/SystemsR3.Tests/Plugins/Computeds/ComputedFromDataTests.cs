using R3;
using SystemsR3.Extensions;
using SystemsR3.Tests.Plugins.Computeds.Models;
using Xunit;

namespace SystemsR3.Tests.Plugins.Computeds
{
    public class ComputedFromDataTests
    {
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
        public void should_refresh_value_when_refresh_when_is_triggered()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = 15};
            
            var computedData = new TestComputedFromData(data);
            computedData.OnChanged.Subscribe(x => hasNotified = true);
            
            data.Data = expectedData;
            computedData.ManuallyRefresh.OnNext(Unit.Default);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.True(hasNotified);
        }
        
        [Fact]
        public void should_not_notify_when_datasource_not_changed_when_refreshing()
        {
            var expectedData = 20;
            var hasNotified = false;
            var data = new DummyData{Data = expectedData};
            
            var computedData = new TestComputedFromData(data);
            computedData.RefreshData();
            
            computedData.OnChanged.Subscribe(x => hasNotified = true);
            computedData.RefreshData();

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);
            Assert.False(hasNotified);
        }
    }
}