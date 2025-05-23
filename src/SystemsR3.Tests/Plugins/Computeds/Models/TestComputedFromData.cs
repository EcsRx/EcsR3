using R3;
using SystemsR3.Computeds.Conventions;

namespace SystemsR3.Tests.Plugins.Computeds.Models
{
    public class TestComputedFromData : ComputedFromData<int, DummyData>
    {
        public Subject<Unit> ManuallyRefresh = new Subject<Unit>();
        
        public TestComputedFromData(DummyData data) : base(data)
        {}

        protected override Observable<Unit> RefreshWhen()
        { return ManuallyRefresh; }

        protected override void UpdateComputedData()
        { ComputedData = DataSource.Data; }
    }
}